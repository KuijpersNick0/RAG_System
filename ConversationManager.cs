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

            var exemples =
            @"  User > I choose Sams.EventListener.ActiveMQ.Consumer
            Assistant > I will now solve the final connector's configuration with interleaving Thought, Action, Observation steps : 
            Thought 1: I need to search Sams.EventListener.ActiveMQ.Consumer, find the configuration parameters that are needed, then recommand these parameters values.
            Action 1: Search[Sams.EventListener.ActiveMQ.Consumer]
            Observation 1: The Sams.EventListener.ActiveMQ.Consumer is an EventListerner that has 6 different configuration parameters : Host, Port, Address, UserName, Password, Root, Selector.
            Thought 2: I don't have enough information to fill in the configuration parameters.
            Action 2: Ask[configuration parameters]
            Observation 2: (Result 1 / 1) The Host, Port, Address, UserName, Password, Root, Selector parameters are known. 
            Thought 3: I know the configuration parameters variables values of the chosen connector.
            Action 3: Finish[Host: localhost, Port: 61616, Address: tcp://localhost:61616, UserName: admin, Password: admin, Root: /, Selector: None]";
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


            ("""
                You are a serious engineer assistant who likes to follow the rules. You will help the user to configure the data broker tool. The data broker tool is
                a tool that allows the user to connect to different data sources and destinations. It uses four steps to do this : EventListener, Source, Processor, Destination. 
                At each one of these steps one or multiple connectors can be used. Each connector has a specific configuration.
                You will need to recommand a connector based on the information from the user but 
                you will solve the final connector's configuration with interleaving Thought, Action, Observation steps.

                Thought can reason about the current situation, and Action can be four types:
                (1) Search[Entity], which searches the entity in the memory collection "Sams_Documentation" and returns the first text container if it exists. 
                If not, it will return some similar entities to search.
                (2) Lookup[keyword], which returns the next sentence containing keyword in the current passage.
                (3) Ask[question], which asks the user a question and returns the answer.
                (4) Finish[answer], which returns the answer and finishes the task.
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