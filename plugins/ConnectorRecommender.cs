using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning.Handlebars;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Memory;

// using Microsoft.SemanticKernel.Abstractions; 

namespace Plugins;

// Later on, inherit from : IPlugin (create interface class for robusteness)
public class ConnectorRecommender 
{
    private readonly ILogger _logger;

    public ConnectorRecommender(ILoggerFactory loggerFactory)
    {
        this._logger = loggerFactory.CreateLogger<ConnectorRecommender>();
    }

    [KernelFunction]
    [Description("Recommands a connector based on the user's request")]
    [return: Description("The connector to use based on the process advancement and the user's request")] 
    public  async Task<string> SolveAsync(
        Kernel kernel,
        [Description("The user's description about how data is transmitted between internal and external systems. Based on this information and at which step we are in the process, connectors can be chosen")] string request
    ) 
    {
        // 1. Ask User for Process Step
        var processStepResult = await kernel.InvokePromptAsync("What is the current step in your process?");
        var processStep = processStepResult?.ToString()?.Trim();

        if (string.IsNullOrEmpty(processStep))
        {
            return "Unable to determine the process step. Please provide a valid process step.";
        }

        // 2. Invoke ConnectorPlugin to Get Connectors
        var connectorsResult = await kernel.InvokeAsync<List<string>>("ConnectorPlugin.getConnectorsStep", processStep);
        var connectors = connectorsResult;

         // 3. Handle Connector List
        if (connectors == null || connectors.Count == 0)
        {
            return "No connectors available for the given process step.";
        }

        // 4. Ask User for Connector Preferences (optional)

        // 5. Recommend Connector
        var recommendedConnector = RecommendConnectorBasedOnPreferences(request, connectors);

        // 6. Return Recommendation
        return $"Based on your process step, I recommend using the connector: {recommendedConnector}. Is there anything else you need?";
    }

    // Method to handle additional logic for recommending a connector based on user preferences
    private string RecommendConnectorBasedOnPreferences(string request, List<string> connectors )
    {
         // Your logic to recommend a connector based on the user's request and the list of possible connectors
        // Implement your recommendation algorithm here
    
        // For now, let's just return the first connector in the list (you should replace this with your logic)
        return connectors.FirstOrDefault();
    }
}