using System;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Memory;
using Microsoft.SemanticKernel.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

using Microsoft.SemanticKernel.Connectors.Chroma;

using Microsoft.SemanticKernel.Planning.Handlebars;
using Plugins;

using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Azure.AI.OpenAI;
using Json.More;

using Smart_Sams;

namespace Plugins
{
    public class DatabasePlugin
    {
        private readonly string MemoryCollectionName = "Sams_Documentation";
        private readonly ISemanticTextMemory memory;

        public DatabasePlugin()
        {
            // I need to initialize the memory
            var memoryBuilder = DatabaseInitializer.InitializeMemory();
            memory = memoryBuilder.Build();
        }

        [KernelFunction]
        [Description("Returns the connector information based on the query and metadata field requested out of the memory")]
        [return: Description("The connector information")]
        public async Task<String> GetWithConnectorName(
            [Description("The connector that the user has asked to configure")] string query, 
            Kernel kernel, 
            [Description("The information that is needed out of the metadata to configure the connector")] string metadataField = "text")
        {
            var response = await memory.GetAsync(MemoryCollectionName, query, true, kernel);

            string result = string.Empty;

            if (response != null && response.Embedding.HasValue)
            {
                // Convert the Span to an array
                var embeddingArray = response.Embedding.Value.ToArray();

                // Assuming there's text information in the metadata, print the specified field
                if (response.Metadata != null)
                {
                    switch (metadataField.ToLower())
                    {
                        case "text":
                            result = $"Text: {response.Metadata.Text}";
                            break;
                        case "description":
                            result = $"Description: {response.Metadata.Description}";
                            break;
                        default:
                            Console.WriteLine($"Unknown metadata field: {metadataField}, please add to the plugin DatabasePlugin.cs");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("No metadata available");
                }
            }
            return result;
        }
        
        public async Task<String> GetSemanticSearch(string query, Kernel kernel, string metadataField = "text")
        {
            var response = memory.SearchAsync(MemoryCollectionName, query, limit: 1, minRelevanceScore: 0.1, true);
            string result = string.Empty;

            // Handle the search response as needed 
            await foreach (var r in response)
            {
                switch (metadataField.ToLower())
                {
                    case "text":
                        result = $"Search result: {r.Metadata.Text}";
                        break;
                    case "description":
                        result = $"Search result description: {r.Metadata.Description}";
                        break;
                    default:
                        Console.WriteLine($"Unknown metadata field: {metadataField}, please add to the plugin DatabasePlugin.cs");
                        break;
                }
            }
            return result;
        }
    }
}