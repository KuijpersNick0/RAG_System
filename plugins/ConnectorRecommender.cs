using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Memory;
using HandlebarsDotNet;

// using Microsoft.SemanticKernel.Abstractions; 

namespace Plugins;

// Later on, inherit from : IPlugin (create interface class for robusteness)
public class ConnectorRecommender
{
    [KernelFunction]
    [Description("Returns back the required steps necessary to recommand a connector")]
    [return: Description("The connector configuration parameters to use")]
    public async Task<string> SolveAsync(
        Kernel kernel,
        [Description("Request for a recommandation of a connector")] string request
    )
    {
        var exemples =
        // Possibility to have examples in vector database ? Reduce token size. 
        // Integrate metadata to search method to facilitate the search.
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

        // var result = await kernel.InvokePromptAsync($"""
        // I need to recommand a connector based on this information {request} from the user but
        // before I do that, I need to ask the user for the current step in the process.
        // Based on the current step, I will then invoke the ConnectorPlugin getConnectorsStep function to get the connectors available at this step.
        // I will then ask the user to chose one connector from the list of connectors available.
        // Finally, I will solve the final connector's configuration with interleaving Thought, Action, Observation steps.

        // Thought can reason about the current situation, and Action can be four types:
        // (1) Search[Entity], which searches the entity in the memory collection "SAMSDocumentation" and returns the first text container if it exists. 
        // If not, it will return some similar entities to search.
        // (2) Lookup[keyword], which returns the next sentence containing keyword in the current passage.
        // (3) Ask[question], which asks the user a question and returns the answer.
        // (4) Finish[answer], which returns the answer and finishes the task.


        // """, new() {
        //     { "request", request },
        //     {"exemples", exemples} 
        // });

        // return result?.ToString();



        // 1. Ask User for Process Step
        Console.Write("What is the current step in your process? ");
        var processStep = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(processStep))
        {
            return "Unable to determine the process step. Please provide a valid process step.";
        }

        // 2. Instantiate ConnectorPlugin and Get Connectors
        var connectorPlugin = new ConnectorPlugin();
        var connectorsResult = connectorPlugin.getConnectorsStep(processStep);

        // 3. Handle Connector List error
        if (connectorsResult == null || connectorsResult.Count == 0)
        {
            return "No connectors available for the given process step.";
        }

        foreach (var connector in connectorsResult)
        {
            Console.WriteLine(connector);
        }
        // 4. Ask User for Connector Preferences 
        Console.WriteLine("Which connector should we keep out of the list of connectors available?");
        var connectorPreference = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(connectorPreference))
        {
            return "Unable to determine the connector preferences. Please provide a valid connector preference.";
        }

        Console.WriteLine("Continuing with the connector: " + connectorPreference);

        // 5. Get context out of database linked to the chosen connector 
        var databasePlugin = new DatabasePlugin();
        var databaseResultText = await databasePlugin.getWithConnectorName(connectorPreference, kernel, "text");

        // 6. Make ReAct prompt to solve the final connector's configuration
        var result = await kernel.InvokePromptAsync($"""
        I need to recommand a connector based on this information {request} from the user but 
        I will solve the final connector's configuration with interleaving Thought, Action, Observation steps.

        Thought can reason about the current situation, and Action can be four types:
        (1) Search[Entity], which searches the entity in the memory collection "SAMSDocumentation" and returns the first text container if it exists. 
        If not, it will return some similar entities to search.
        (2) Lookup[keyword], which returns the next sentence containing keyword in the current passage.
        (3) Ask[question], which asks the user a question and returns the answer.
        (4) Finish[answer], which returns the answer and finishes the task.

        context : {databaseResultText}

        This are the exemples :
        {exemples}
        
        """, new() {
            { "request", request },
            {"exemples", exemples},
            { "context", databaseResultText}
        });

        return result?.ToString();
    }
}