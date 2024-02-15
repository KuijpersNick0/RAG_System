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
    private readonly ILogger<ConnectorRecommender> _logger;
    public ConnectorRecommender(ILogger<ConnectorRecommender> logger)
    {
        _logger = logger;
    }


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
        @"Assistant:
            Thought 1: I need to search for information about Sams.EventListener.ActiveMQ.Consumer to find the required configuration parameters.
            Action 1: Search[Sams.EventListener.ActiveMQ.Consumer]
            Observation 1: After returned information from the database search I know that Sams.EventListener.ActiveMQ.Consumer is an EventListener with 
            6 configuration parameters: Host, Port, Address, UserName, Password, Root, Selector.
            Thought 2: I know what the description of these parameters are, their default values but not the definite values chosen by the user. I shall 
            ask the user for these values.
            Action 2: Ask[Do you need to configure the Host, Port, Address, UserName, Password, Root, Selector parameters?]
        User response: 
            Yes, I need all of them except Selector. 
        Assistant:    
            Observation 2: The parameters Host, Port, Address, UserName, Password, Root are demanded.
            Thought 3: I shall ask the user for the values of these parameters.
            Action 3: Ask[The default value of Host is localhost, do you want to change it?]
        User response: 
            yes, change to 124:15:15:15
        Assistant:
            Observation 3: The value of Host is 124:15:15:15.
            Thought 4: I shall ask the user for the value of the Port parameter.
            Action 4: Ask[The default value of Port is 61616, do you want to change it?]
        User response: 
            No
        Assistant:
            Observation 4: The value of Port is 61616.
            Thought 5: I shall ask the user for the value of the Address parameter.
            Action 5: Ask[The default value of Address is tcp://localhost:61616, do you want to change it?]
        User response: 
            No I don't need to change it.
        Assistant:
            Observation 5: The value of Address is tcp://localhost:61616.
            Thought 6: I shall ask the user for the value of the UserName parameter.
            Action 6: Ask[The default value of UserName is admin, do you want to change it?]
        User response: 
            No 
        Assistant:
            Observation 6: The value of UserName is admin.
            Thought 7: I shall ask the user for the value of the Password parameter.
            Action 7: Ask[The default value of Password is admin, do you want to change it?]
        User response:
            No
        Assistant:
            Observation 7: The value of Password is admin.
            Thought 8: I shall ask the user for the value of the Root parameter.
            Action 8: Ask[The default value of Root is Root, do you want to change it?]
        User response: 
            No
        Assistant:
            Observation 8: The value of Root is Root.
            Thought 9: I have all the values of the demanded parameters. I shall finish the task.
            Action 9: Finish[Host: 124:15:15:15, Port: 61616, Address: tcp://localhost:61616, UserName: admin, Password: admin, Root: Root, Selector: None]";

        // 1. Ask User for Process Step
        Console.Write("What is the current step in your process? ");
        var processStep = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(processStep))
        {
            Console.WriteLine("Unable to determine the process step. Please provide a valid process step.");
        }

        // 2. Instantiate ConnectorPlugin and Get Connectors
        var connectorPlugin = new ConnectorPlugin();
        var connectorsResult = connectorPlugin.getConnectorsStep(processStep);

        // 3. Handle Connector List error
        if (connectorsResult == null || connectorsResult.Count == 0)
        {
            Console.WriteLine("No connectors available for the given process step.");
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
            Console.WriteLine("Unable to determine the connector preferences. Please provide a valid connector preference.");
        }

        Console.WriteLine("Continuing with the connector: " + connectorPreference);

        // 5. Get context out of database linked to the chosen connector 
        var databasePlugin = new DatabasePlugin();
        var databaseResultText = await databasePlugin.getWithConnectorName(connectorPreference, kernel, "text");

        // 6. Make ReAct prompt to solve the final connector's configuration
        var result = await kernel.InvokePromptAsync($"""
        I need to help configure the connector : {connectorPreference} the user chose but 
        I will solve the final connector's configuration with interleaving Thought, Action, Observation steps.

        Thought can reason about the current situation, and Action can be two types:
        (1) Ask[question], which asks the user a question and waits for it's answer before continuing.
        (2) Finish[answer], which returns the answer and finishes the task.

        This is an exemple of proceding to recommand a connector configuration. Assistant is you, the user is me you should not answer your own questions:
        {exemples}

        This is the context of the connector configuration provided by a search through the database:
        {databaseResultText}

        """, new() {
            {"connectorPreference", connectorPreference},
            {"exemples", exemples},
            {"context", databaseResultText}
        });

        // Log the model's response
        // This works
        // _logger.LogInformation("Model's response: {ModelResponse}", result);


        return result?.ToString();
    }
}