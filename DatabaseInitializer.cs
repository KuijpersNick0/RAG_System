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
  
using System.Xml;

using System.Text;

namespace Smart_Sams
{
    public class DatabaseInitializer
    { 
        public static MemoryBuilder InitializeMemory(string endpoint, string apiKey)
        {
            var memoryBuilder = new MemoryBuilder();
            memoryBuilder.WithAzureOpenAITextEmbeddingGeneration(
                "text-embedding-ada-002",
                endpoint,
                apiKey,
                "model-id");

            // You can easily change the connector to db here
            var chromaMemoryStore = new ChromaMemoryStore("http://127.0.0.1:8000");
            memoryBuilder.WithMemoryStore(chromaMemoryStore);
 

            return memoryBuilder;
        }

        // private async Task<string> SearchMemoriesAsync(Kernel kernel, MemoryBuilder memory, string query)
        // {
        //     StringBuilder result = new StringBuilder();
        //     result.Append("The below is relevant information.\n[START INFO]");
            
        //     // Search for memories that are similar to the user's input.
        //     const string memoryCollectionName = "Sams_Documentation_v2"; 
            
        //     IAsyncEnumerable<MemoryQueryResult> queryResults = 
        //         memory.SearchAsync(memoryCollectionName, query, limit: 3, minRelevanceScore: 0.77);

        //     // For each memory found, try to get previous and next memories.
        //     await foreach (MemoryQueryResult r in queryResults)
        //     {
        //         int id = int.Parse(r.Metadata.Id);
        //         MemoryQueryResult? rb2 = await kernel.Memory.GetAsync(memoryCollectionName, (id - 2).ToString());
        //         MemoryQueryResult? rb = await kernel.Memory.GetAsync(memoryCollectionName, (id - 1).ToString());
        //         MemoryQueryResult? ra = await kernel.Memory.GetAsync(memoryCollectionName, (id + 1).ToString());
        //         MemoryQueryResult? ra2 = await kernel.Memory.GetAsync(memoryCollectionName, (id + 2).ToString());

        //         if (rb2 != null) result.Append("\n " + rb2.Metadata.Id + ": " + rb2.Metadata.Description + "\n");
        //         if (rb != null) result.Append("\n " + rb.Metadata.Description + "\n");
        //         if (r != null) result.Append("\n " + r.Metadata.Description + "\n");
        //         if (ra != null) result.Append("\n " + ra.Metadata.Description + "\n");
        //         if (ra2 != null) result.Append("\n " + ra2.Metadata.Id + ": " + ra2.Metadata.Description + "\n");
        //     }

        //     result.Append("\n[END INFO]");
        //     result.Append($"\n{query}");

        //     return result.ToString();
        // } 
    }
}
