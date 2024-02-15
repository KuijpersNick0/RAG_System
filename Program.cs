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

            // Logging
            var logFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));
            builder.Services.AddSingleton<ILoggerFactory>(logFactory);
            builder.Services.AddSingleton(logFactory.CreateLogger<ConnectorRecommender>());


            // builder.Plugins.AddFromType<ConnectorPlugin>();

            var kernel = builder.Build();
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();


            var memoryBuilder = DatabaseInitializer.InitializeMemory();
            var memory = memoryBuilder.Build();

            // var memoryPlugin = kernel.ImportPluginFromObject(new TextMemoryPlugin(memory));

            var conversationManager = new ConversationManager(chatCompletionService, new ChatHistory(), kernel);
            await conversationManager.StartConversation();
        }
    }
}
