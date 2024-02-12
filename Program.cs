using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
using Microsoft.SemanticKernel.Connectors.Chroma;

using Microsoft.SemanticKernel.Planning.Handlebars;
using Plugins;

using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Azure.AI.OpenAI;
using Json.More;

namespace Smart_Sams
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Kernel.CreateBuilder();
            builder.WithCompletionService();
            builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));

            // Helps with the history : To summarize a conversation : less tokens
            builder.Plugins.AddFromType<ConversationSummaryPlugin>();
            // Plannerbehind the connectors functionnality calling
            builder.Plugins.AddFromType<ConnectorRecommender>();

            // builder.Plugins.AddFromType<ConnectorPlugin>();

            var kernel = builder.Build();
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            var endpoint = configuration["AzureOpenAI:Endpoint"];
            var apiKey = configuration["AzureOpenAI:ApiKey"];

            var memoryBuilder = DatabaseInitializer.InitializeMemory(endpoint, apiKey);
            var memory = memoryBuilder.Build();

            var MemoryCollectionName = "Sams_Documentation_v2";

            var questions = new[]
            {
                // "What listens to Appach Active MQ ?",
                "Sams.Connector.Reporting.Destination.Rdlc",
                "What is the Assembly information of Sams.Connector.Reporting.Destination.Rdlc ?",
                // "What is the description of Sams.Connector.Reporting.Destination.Docx ?",  
            };

            // foreach (var q in questions)
            // {
            //     // var response = memory.SearchAsync(MemoryCollectionName, q, limit: 1, minRelevanceScore: 0.1, true);
            //     // Console.WriteLine(q + " response: " + response?.Metadata.Text);

            //     var response = await memory.GetAsync(MemoryCollectionName, q, true, kernel);

            //     // var rep = await response;
            //     Console.WriteLine(response?.Embedding.Value.Span[0].ToString());
            //     // await foreach (MemoryQueryResult r in response)
            //     // {
            //     //     // Console.WriteLine(r.Embedding.Value + " " + r.Metadata.Id + " " +  r.Metadata.Text + " " + r.Metadata.Description + " " + r.Metadata.AdditionalMetadata + " " + r.Relevance);
            //     //     // var embedding = r.Embedding.Value;
            //     //     // var message = Encoding.UTF8.GetString(embedding);

            //     //     if (r.Embedding.HasValue)
            //     //     {
            //     //         Console.WriteLine(r.Embedding.Value.Span[0].ToString());
            //     //     }

            //     // }

            //     // Console.WriteLine(await response.Metadata.Id + " " + await response.Metadata.Text + " " + await response.Relevance);



            //     // if (Embeddings.HasValue)
            //     // {
            //     //     float[] embeddingsArray = Embeddings.Value.ToArray();
            //     //     string embeddingsString = string.Join("a, ", embeddingsArray);
            //     //     Console.WriteLine(embeddingsString);
            //     // }
            // }

            foreach (var q in questions)
            {
                var response = await memory.GetAsync(MemoryCollectionName, q, true, kernel);

                if (response != null && response.Embedding.HasValue)
                {
                    // Convert the Span to an array
                    var embeddingArray = response.Embedding.Value.ToArray();

                    // Print the entire embedding vector
                    Console.WriteLine($"Embedding Vector: {string.Join(", ", embeddingArray)}");

                    // Assuming there's text information in the metadata, print it
                    if (response.Metadata != null)
                    {
                        Console.WriteLine($"Text: {response.Metadata.Text}");
                    }
                    else
                    {
                        Console.WriteLine("No metadata available");
                    }
                }
            }

            var memoryPlugin = kernel.ImportPluginFromObject(new TextMemoryPlugin(memory));

            var conversationManager = new ConversationManager(chatCompletionService, new ChatHistory(), kernel);
            await conversationManager.StartConversation();
        }
    }
}
