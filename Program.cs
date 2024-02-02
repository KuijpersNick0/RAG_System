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
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Kernel.CreateBuilder();
            builder.WithCompletionService();
            builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));

            builder.Plugins.AddFromType<ConversationSummaryPlugin>();
            builder.Plugins.AddFromType<ConnectorRecommender>();
            builder.Plugins.AddFromType<ConnectorPlugin>();

            var kernel = builder.Build();
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();

            var endpoint = configuration["AzureOpenAI:Endpoint"];
            var apiKey = configuration["AzureOpenAI:ApiKey"];

            var memoryBuilder = DatabaseInitializer.InitializeMemory(endpoint, apiKey);
            var memory = memoryBuilder.Build();

            var memoryPlugin = kernel.ImportPluginFromObject(new TextMemoryPlugin(memory));

            var conversationManager = new ConversationManager(chatCompletionService, new ChatHistory(), kernel);
            await conversationManager.StartConversation();
        }
    }
}
