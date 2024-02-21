using System.ComponentModel;
using Azure;
using Microsoft.SemanticKernel; 
using Microsoft.SemanticKernel.ChatCompletion;

namespace Plugins;
public class BasePlugin 
{
    private readonly ChatHistory history;

    public BasePlugin(ChatHistory history)
    {
        this.history = history;
    }

    [KernelFunction]
    [Description("Ask the user for more information")]
    [return: Description("The user response")]
    public Task<string> AskUser( 
        [Description("The question to ask the user")] string question
    )
    {   
        Console.WriteLine(question);  
        history.AddAssistantMessage(question); //Adding them to the history is probably a good idea
        string response = Console.ReadLine() ?? "Not a valid response, ask again";  
        history.AddUserMessage(response);
        return Task.FromResult(response);
    }
}