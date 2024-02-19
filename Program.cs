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
            // Helps with recommandation steps : Didn't use this as giving too much liberty to the LLM will make it hallucinate or give wrong recommandations
            // builder.Plugins.AddFromType<ConnectorRecommender>();  
            // Helps with the memory   
            builder.Plugins.AddFromType<DatabasePlugin>();
            // Helps with the connectors functions and generation
            builder.Plugins.AddFromType<ConnectorPlugin>();
            // Helps the LLM ask a explicit question to the user, makes the LLM buggy, I don't know why : choses himself the connectors out of list
            // builder.Plugins.AddFromType<BasePlugin>();

            // Logging
            var logFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));
            builder.Services.AddSingleton<ILoggerFactory>(logFactory);
            builder.Services.AddSingleton(logFactory.CreateLogger<ConnectorPlugin>());
            
            var EventListenerFactory = new EventListenerFactory();
            builder.Services.AddSingleton<IArtifactFactory>(EventListenerFactory);

            var kernel = builder.Build();
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            // Initialize the memory with connection to the database
            var memoryBuilder = DatabaseInitializer.InitializeMemory();
            var memory = memoryBuilder.Build();

            // Import the memory plugin to the kernel -> He uses it when not needed (for the moment I don't give it access to the memory by default)
            // var memoryPlugin = kernel.ImportPluginFromObject(new TextMemoryPlugin(memory));

            // Initialize the conversation manager
            var conversationManager = new ConversationManager(chatCompletionService, new ChatHistory(), kernel);
            await conversationManager.StartConversation();
        }
    }
}
