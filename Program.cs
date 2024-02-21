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
using System.Reflection.Metadata;

using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks; 
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SemanticKernel.Plugins.Document;

namespace Smart_Sams
{
    public class Program
    {   
        // This function helps the user to configure the data broker tool with connector configuration
        public static async Task ConfigureDataBrokerTool()
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

            // // Generate the XML file when needed
            // var XmlGenerationPluginPath = "C:/Users/z000p01m/Documents/Stage/code/RAG_System_V3/RecommandationSystem/plugins/Prompts/XmlGeneration"; 
            // kernel.ImportPluginFromPromptDirectory(XmlGenerationPluginPath);
             
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            
            // Initialize the memory with connection to the database
            var memoryBuilder = DatabaseInitializer.InitializeMemory();
            var memory = memoryBuilder.Build();

            // Import the memory plugin to the kernel -> He uses it when not needed (for the moment I don't give it access to the memory by default)
            // var memoryPlugin = kernel.ImportPluginFromObject(new TextMemoryPlugin(memory));

            // Initialize the conversation manager
            var conversationManager = new ConversationManager(chatCompletionService, new ChatHistory(), kernel, "1");
            await conversationManager.StartConversation();
        }

        // This function helps the user to ask for XSLT transformation files
        public static async Task XSLTTransformationFiles()
        { 
            var builder = Kernel.CreateBuilder();
            builder.WithCompletionService();
            builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));

            // TextPlugin
            // FileIOPlugin

            // DocumentPlugin documentPlugin = new(new WordDocumentConnector(), new LocalDriveConnector());
            // string filePath = "PATH_TO_DOCX_FILE.docx";
            // string text = await documentPlugin.ReadTextAsync(filePath);
            // Console.WriteLine(text);
            // builder.Plugins.AddFromType<DocumentPlugin>();
            
            builder.Plugins.AddFromType<TransformationPlugin>();

            // Logging
            var logFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Trace));
            builder.Services.AddSingleton<ILoggerFactory>(logFactory);
            builder.Services.AddSingleton(logFactory.CreateLogger<TransformationPlugin>());

            var kernel = builder.Build();
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

            // Initialize the conversation manager
            var conversationManager = new ConversationManager(chatCompletionService, new ChatHistory(), kernel, "2");
            await conversationManager.StartConversation();
        }

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Welcome user ! I am Smart Sams, your assistant for the data broker tool. I will help you configure the data broker tool.");
            Console.WriteLine("For the moment I have 2 services to offer : 1. Configure the data broker tool with connector configuration and 2. Ask for XSLT transformation files.");
            Console.WriteLine("What do you want to do ? 1 or 2 ?");
            string response = Console.ReadLine() ?? string.Empty;
            if (response == "1")
            {
                await ConfigureDataBrokerTool();
            }
            else if (response == "2")
            {
                await XSLTTransformationFiles();
            }
            else
            {
                Console.WriteLine("Not a valid response, ask again");
            }

        }
    }
}
