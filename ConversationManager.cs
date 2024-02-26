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
    // This the main class managing the conversation flow
    public class ConversationManager
    {
        // Dependencies for ConversationManager
        private readonly IChatCompletionService chatCompletionService;
        private readonly ChatHistory history;
        private readonly Kernel kernel;
        private readonly string? choice;

        public ConversationManager(IChatCompletionService chatCompletionService, ChatHistory history, Kernel kernel, string? choice)
        {
            // Initialize chat history with initial instructions -> gives "personality" to the assistant
            if (choice == "1"){
                ChatHistory chatMessages = new ChatHistory
                ("""
                You are a serious assistant who likes to follow the rules. You will help the user to configure the data broker tool. The data broker tool 
                allows the user to connect internal and external data sources through connectors. There are four steps in the data broker tool: EventListener, Source, Processor and Destination.
                Each process step require a connector or multiple connectors to configure. A connector is described by Description (String), Assembly Informations (String), Properties (Dictionary<string, string>), Attributes (Dictionary<string, AttributeInfo>) and Configuration (Dictionary<string, ConfigurationInfo>).
                This is how you will proceed :
                1. Ask at which step the user is in the process of configuring the data broker tool.
                2. Provide the user a list of connectors that are possible to choose from based on the step. NEVER chose the connector yourself !
                3. Ask the user which connector he want to configure. Do not go further if the user doesn't provide the connector name he wants to configure. 
                4. Present the user the information about the chosen connector.
                5. Ask the user which configuration parameters of the connector he wants to keep.
                4. For the configuration parameters needed, if it was not provided before, ask what value the user wants to give to the configuration parameter.
                5. Save the connector object.
                You will complete required steps and request approval before taking any consequential actions. If the user doesn't provide enough information for you to complete a task, you will keep asking questions until you have enough information to complete the task. Do not hallucinate.
                Here are the main functions you should use:
                (1) GetConnectorsStepList : Returns a list of possible connectors the user can choose from based on the current process step of the data broker tool.
                (2) GetWithConnectorName : Returns the connector information based on the query and metadata field (choose text) requested out of the memory. 
                (3) CreateConnectorObject : Creates a connector object, this should be invoked after we have all the configuration parameters of the connector.
                (4) GetCreatedConnectorsConfigurationVariables : Returns the list of connectors and their configuration parameters so far created.  
                Proceed step by step.
                """);
                this.chatCompletionService = chatCompletionService;
                this.history = chatMessages;
                this.kernel = kernel;
            } else if (choice == "2") {
                ChatHistory chatMessages = new ChatHistory
                ("""
                You are a seriours assistant who likes to follow the rules. You will help the user generate XSLT code. XSLT is a language for transforming XML documents into other XML documents.
                The user will provide you with the source XML document and the target XML document. You will ask the user for the transformation rules and generate the XSLT code. The rules are defined 
                by the user and are based on the user's knowledge of the required output. You will complete required steps and request approval before taking any consequential actions. If the user doesn't provide enough information for you to complete a task, you will keep asking questions until you have enough information to complete the task. Do not hallucinate.
                """);
                this.chatCompletionService = chatCompletionService;
                this.history = chatMessages;
                this.kernel = kernel;
            } else {
                throw new System.ArgumentException("Invalid choice", "choice");
            }
        }

        // Method to start the conversation loop
        public async Task StartConversation()
        {
            while (true)
            {
                // Prompt user input
                Console.Write("User > ");
                history.AddUserMessage(Console.ReadLine()!);


                // Settings for OpenAI prompt execution
                OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
                {
                    // Set the temperature to 0.5 to make the assistant's responses more conservative [0-1], 1 being the most random (default)
                    Temperature = 0.5, 
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
                };

                // Get streaming chat message contents asynchronously
                var result = chatCompletionService.GetStreamingChatMessageContentsAsync(
                    history,
                    executionSettings: openAIPromptExecutionSettings,
                    kernel: kernel);

                // String to store the history 
                string fullMessage = "";
                var first = true;
                // Iterate through the results
                await foreach (var content in result)
                {
                    if (content.Role.HasValue && first)
                    {
                        Console.Write("Assistant > ");
                        first = false;
                    }
                    Console.Write(content.Content);
                    fullMessage += content.Content;
                }


                Console.WriteLine();
                history.AddAssistantMessage(fullMessage);
            }
        }
    }
}