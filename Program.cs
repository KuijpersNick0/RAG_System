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
using classes;

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
            // Helps with recommandation steps
            // builder.Plugins.AddFromType<ConnectorRecommender>();  
            // Helps with the memory   
            builder.Plugins.AddFromType<DatabasePlugin>();
            // Helps with the connectors functions and generation
            builder.Plugins.AddFromType<ConnectorPlugin>();

            // Logging
            var logFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));
            builder.Services.AddSingleton<ILoggerFactory>(logFactory);
            builder.Services.AddSingleton(logFactory.CreateLogger<ConnectorRecommender>());
            
            var EventListenerFactory = new EventListenerFactory();
            builder.Services.AddSingleton<IArtifactFactory>(EventListenerFactory);

            var kernel = builder.Build();
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            // Initialize the memory with connection to the database
            var memoryBuilder = DatabaseInitializer.InitializeMemory();
            var memory = memoryBuilder.Build();

            // Import the memory plugin to the kernel
            var memoryPlugin = kernel.ImportPluginFromObject(new TextMemoryPlugin(memory));

            // Initialize the conversation manager
            var conversationManager = new ConversationManager(chatCompletionService, new ChatHistory(), kernel);
            await conversationManager.StartConversation();
        }
    }
}
