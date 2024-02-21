using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.ChatCompletion;

using HandlebarsDotNet;
using classes;


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
    [Description("Is responsible of configuring the connector")]
    [return: Description("The connector configuration")]
    public async Task<string> SolveConnectorConfiguration(
        Kernel kernel,
        [Description("The request for configuring of a connector")] string request
    )
    {
        var exemples =
        @"Assistant:
            Thought 1: The user wants to configure a connector called Sams.EventListener.ActiveMQ.Consumer.
            Observation 1: I don't know Sams.EventListener.ActiveMQ.Consumer information.
            Action 1: Search[Sams.EventListener.ActiveMQ.Consumer]
            Observation 1: I know that Sams.EventListener.ActiveMQ.Consumer is an EventListener with 6 configuration parameters: Host, Port, Address, UserName, Password, Root, Selector.
            Thought 2: I know what the description of these parameters are, their default values but not the definite values chosen by the user. I shall 
            ask the user for these values.
            Action 2: Ask[Do you need to configure the Host, Port, Address, UserName, Password, Root, Selector parameters ?]
        User response: 
            yes 
        Assistant:    
            Observation 2: The parameters Host, Port, Address, UserName, Password, Root are demanded.
            Thought 3: I shall ask the user for the values of these parameters.
            Action 3: Ask[The default value of Host is localhost, do you want to change it ?]
        User response: 
            yes
        Assistant:
            Observation 3: The value of Host cannot be localhost as the user wants to change it.
            Thought 4: I must ask the new value of Host
            Action 4: Ask[What value do you want for Host ?]
        User response:
            124:15:15:15
        Assistant:
            Observation 3: The value of Host is 124:15:15:15.
            Thought 4: I shall ask the user for the value of the Port parameter.
            Action 4: Ask[The default value of Port is 61616, do you want to change it ?]
        User response: 
            No
        Assistant:
            Observation 4: The value of Port is 61616.
            Thought 5: I shall ask the user for the value of the Address parameter.
            Action 5: Ask[The default value of Address is tcp://localhost:61616, do you want to change it ?]
        User response: 
            No
        Assistant:
            Observation 5: The value of Address is tcp://localhost:61616.
            Thought 6: I shall ask the user for the value of the UserName parameter.
            Action 6: Ask[The default value of UserName is admin, do you want to change it ?]
        User response: 
            No 
        Assistant:
            Observation 6: The value of UserName is admin.
            Thought 7: I shall ask the user for the value of the Password parameter.
            Action 7: Ask[The default value of Password is admin, do you want to change it ?]
        User response:
            No
        Assistant:
            Observation 7: The value of Password is admin.
            Thought 8: I shall ask the user for the value of the Root parameter.
            Action 8: Ask[The default value of Root is Root, do you want to change it ?]
        User response: 
            No
        Assistant:
            Observation 8: The value of Root is Root.
            Thought 9: I have all the values of the demanded parameters. I shall finish the task.
            Action 9: Finish[Host: 124:15:15:15, Port: 61616, Address: tcp://localhost:61616, UserName: admin, Password: admin, Root: Root, Selector: None]";

        // Make ReAct prompt to solve the final connector's configuration
        var result = await kernel.InvokePromptAsync
        ($"""
        I need to help the user to configure connectors but 
        I will solve the final connector's configuration with interleaving Thought, Action, Observation steps.

        Thought can reason about the current situation, and Action can be three types:
        (1) Search[query], which searches the database for the query in the Sams_Documentation collection memory and returns the result.
        (2) Ask[question], which asks the user a question and waits for it's answer before continuing.
        (3) Finish[answer], which returns the answer and finishes the task.

        Assistant is you, the user is me. Use these few-shot exemples as guideline. You should not answer your own questions. 
        These are the few-shot exemples to help you understand the process of configuring a connector: {exemples}
        """, new() { 
            {"exemples", exemples},
        });

        // Log the model's response
        // This works
        _logger.LogInformation("Model's response: {ModelResponse}", result);


        return result.ToString();
    }
}