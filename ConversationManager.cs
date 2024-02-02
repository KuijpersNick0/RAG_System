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
    public class ConversationManager
    {
        private readonly IChatCompletionService chatCompletionService;
        private readonly ChatHistory history;
        private readonly Kernel kernel;

        public ConversationManager(IChatCompletionService chatCompletionService, ChatHistory history, Kernel kernel)
        {
            this.chatCompletionService = chatCompletionService;
            this.history = history;
            this.kernel = kernel;
        }

        public async Task StartConversation()
        {
            while (true)
            {
                Console.Write("User > ");
                history.AddUserMessage(Console.ReadLine()!);

                OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
                {
                    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
                };

                var result = chatCompletionService.GetStreamingChatMessageContentsAsync(
                    history,
                    executionSettings: openAIPromptExecutionSettings,
                    kernel: kernel); 

                string fullMessage = "";
                var first = true;
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