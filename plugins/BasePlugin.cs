using System.ComponentModel;
using Azure;
using Microsoft.SemanticKernel; 

namespace Plugins;
public class BasePlugin 
{
    [KernelFunction]
    [Description("Ask the user for more information")]
    [return: Description("The user response")]
    public Task<string> AskUser(
        [Description("The question to ask the user")] string question
    )
    {   
        Console.WriteLine(question);  
        string response = Console.ReadLine() ?? "Not a valid response, ask again";  
        return Task.FromResult(response);
    }
}