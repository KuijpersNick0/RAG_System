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
        public ConversationManager(IChatCompletionService chatCompletionService, ChatHistory history, Kernel kernel)
        {
            // Initialize chat history with initial instructions -> gives "personality" to the assistant according to doc
            // You are a serious engineer ! Maybe try giving more context about the situation of our problem and what needs to be tackled.

            //     Assistant helps the company employees with their healthcare plan questions, and questions about the employee handbook. Be brief in your answers.
            // Answer ONLY with the facts listed in the list of sources below. If there isn't enough information below, say you don't know. Do not generate answers that don't use the sources below. If asking a clarifying question to the user would help, ask the question.
            // For tabular information return it as an html table. Do not return markdown format. If the question is not in English, answer in the language used in the question.
            // Each source has a name followed by colon and the actual information, always include the source name for each fact you use in the response. Use square brackets to reference the source, for example [info1.txt]. Don't combine sources, list each source separately, for example [info1.txt][info2.pdf].

            ChatHistory chatMessages = new ChatHistory
            // ("""
            // You are a friendly assistant who likes to follow the rules. You will complete required steps
            // and request approval before taking any consequential actions. If the user doesn't provide
            // enough information for you to complete a task, you will keep asking questions until you have
            // enough information to complete the task. 
            // """);
            // ("""
            // You are a serious engineer who likes to follow the rules. You help the user to configure a system by using connectors at each step of the process.
            // You will complete required steps and request approval before taking any consequential actions. If the user doesn't provide
            // enough information for you to complete a task, you will keep asking questions or search the database until you have
            // enough information to complete the task. Only answer if you know the information from the questions asked or the database, do not hallucinate.
            // """);

    //  You need to ask at which step the user is in the process of configuring the data broker tool and then provide a list of connectors that are possible to choose from based on the step.
    //             When the connector is chosen by the user, you need to ask the user for the configuration parameters of the connector. You can then save the connector object with the specified configuration parameters by the user.

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
                Here are the functions you can use:
                (1) GetConnectorsStepList : Returns a list of possible connectors the user can choose from based on the current process step of the data broker tool.
                (2) GetWithConnectorName : Returns the connector information based on the query and metadata field (choose text) requested out of the memory.
                (3) AskUser : Asks the user a question. This is especially useful when you need to ask the user for the configuration parameters of the connector.
                (4) CreateConnectorObject : Creates a connector object, this should be invoked after we have all the configuration parameters of the connector.
                (5) GetCreatedConnectorsConfigurationVariables : Returns the list of connectors and their configuration parameters so far created.  
                Proceed step by step.
            """);

            this.chatCompletionService = chatCompletionService;
            this.history = chatMessages;
            this.kernel = kernel;
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