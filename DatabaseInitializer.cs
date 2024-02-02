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

            var chromaMemoryStore = new ChromaMemoryStore("http://127.0.0.1:8000");
            memoryBuilder.WithMemoryStore(chromaMemoryStore);

            return memoryBuilder;
        }
    }
}
