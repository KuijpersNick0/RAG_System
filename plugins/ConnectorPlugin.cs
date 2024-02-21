using System.ComponentModel;
using Microsoft.SemanticKernel;
using classes;

namespace Plugins;

public class ConnectorPlugin
{
    private readonly IArtifactFactory artifactFactory;
    private readonly List<IArtifact> createdConnectors;

    public ConnectorPlugin(IArtifactFactory artifactFactory)
    {
        this.artifactFactory = artifactFactory;
        this.createdConnectors = new List<IArtifact>(); 
    }

    [KernelFunction]
    [Description("Returns a list of connectors possible to chose from based on which process step the user is in the process of the data broker tool")]
    [return: Description("The list of all the available connectors at the specified process step of the data broker tool")]
    public Task<List<string>> GetConnectorsStepList(
        [Description("The process step at which the user is currently for configuring his data broker tool.")] string step)
    {
        List<string> connectors = new List<string>();
 
        switch (step)
        {
            case "EventListener":
                connectors.Add("_"); // Adding this because for an unkown reason it sometimes proceeds with the first element without asking the user.
                connectors.Add("Sams.EventListener.ActiveMQ.Consumer");
                connectors.Add("Sams.EventListener.AQMessaging.Subscriber");
                connectors.Add("Sams.EventListener.AMQP.Receiver");
                connectors.Add("Sams.EventListener.File.FileSystemWatcher");
                connectors.Add("Sams.EventListener.HTTP.WebServer");
                connectors.Add("Sams.EventListener.Irc.Receive");
                connectors.Add("Sams.EventListener.MQTT.Broker");
                connectors.Add("Sams.EventListener.MQTT.Subscriber");
                connectors.Add("Sams.EventListener.OPC.Subscriber(Customer Connector)");
                connectors.Add("Sams.EventListener.SAP.RfcService");
                connectors.Add("Sams.EventListener.Scheduler.QuartzScheduler");
                connectors.Add("Sams.EventListener.Soap.SecureWebServer");
                connectors.Add("Sams.EventListener.Soap.WebServer");
                connectors.Add("Sams.EventListener.Wcf.RestService");
                connectors.Add("Sams.EventListener.Wcf.SimpleService");
                break;
            case "Source":
                connectors.Add("_");
                connectors.Add("Sams.Connector.Database.Source.Select");
                connectors.Add("Sams.Connector.Database.Source.SelectDb2");
                connectors.Add("Sams.Connector.Database.Source.SelectPG");
                connectors.Add("Sams.Connector.eBR.Source.XsiDatabase");
                connectors.Add("Sams.Connector.eBR.Source.XsiPGDatabase");
                connectors.Add("Sams.Connector.File.Source.Binary");
                connectors.Add("Sams.Connector.File.Source.FileProperties");
                connectors.Add("Sams.Connector.File.Source.Csv");
                connectors.Add("Sams.Connector.File.Source.Json");
                connectors.Add("Sams.Connector.File.Source.Text");
                connectors.Add("Sams.Connector.File.Source.Xml");
                connectors.Add("Sams.Connector.Odata.Source.GetPayload");
                connectors.Add("Sams.Connector.OPC.Source.Read(Customer Connector)");
                connectors.Add("Sams.Connector.SAP.Source.CnxTest");
                connectors.Add("Sams.Connector.SAP.Source.Rfc");
                connectors.Add("Sams.Connector.SAP.Source.Server");
                connectors.Add("Sams.Connector.Sharepoint.Source.LibraryDocument");
                connectors.Add("Sams.Connector.Sharepoint.Source.LibraryDocumentProperties");
                connectors.Add("Sams.Connector.Soap.Source.Http");
                connectors.Add("Sams.Connector.Void.Source.Nop");
                connectors.Add("Sams.Connector.Windows.Source.Wmi");
                break;
            case "Processor":
                connectors.Add("_");
                connectors.Add("Sams.Processor.Database.Execute");
                connectors.Add("Sams.Processor.Database.Select");
                connectors.Add("Sams.Processor.File.Filter");
                connectors.Add("Sams.Processor.OPC.ExecuteMethod(Customer Connector)");
                connectors.Add("Sams.Processor.OPC.Read(Customer Connector)");
                connectors.Add("Sams.Processor.OPC.Write(Customer Connector)");
                connectors.Add("Sams.Processor.Oracle.Execute");
                connectors.Add("Sams.Processor.Oracle.Select");
                connectors.Add("Sams.Processor.Xml.AddMetadata");
                connectors.Add("Sams.Processor.Xml.Merge");
                connectors.Add("Sams.Processor.Xml.XPath");
                connectors.Add("Sams.Processor.Xml.Xsd");
                connectors.Add("Sams.Processor.Xml.Dump");
                connectors.Add("Sams.Processor.Xml.Xsl");
                break;
            case "Destination":
                connectors.Add("_");
                connectors.Add("Sams.Connector.ActiveMQ.Destination.Publisher");
                connectors.Add("Sams.Connector.AMQP.Destination.Sender");
                connectors.Add("Sams.Connector.Database.Destination.Action");
                connectors.Add("Sams.Connector.Database.Destination.Enqueue");
                connectors.Add("Sams.Connector.Database.Destination.Execute");
                connectors.Add("Sams.Connector.eBR.Destination.Do");
                connectors.Add("Sams.Connector.eBR.Destination.DoPG");
                connectors.Add("Sams.Connector.eBR.Destination.eBRDoc");
                connectors.Add("Sams.Connector.eBR.Destination.XsiDatabase");
                connectors.Add("Sams.Connector.eBR.Destination.XsiPGDatabase");
                connectors.Add("Sams.Connector.Email.Destination.SMTP");
                connectors.Add("Sams.Connector.File.Destination.Json");
                connectors.Add("Sams.Connector.File.Destination.MergePdf");
                connectors.Add("Sams.Connector.File.Destination.Move");
                connectors.Add("Sams.Connector.File.Destination.Binary");
                connectors.Add("Sams.Connector.File.Destination.Any");
                connectors.Add("Sams.Connector.File.Destination.Pdf");
                connectors.Add("Sams.Connector.File.Destination.Text");
                connectors.Add("Sams.Connector.HTTP.Destination.Publisher");
                connectors.Add("Sams.Connector.Irc.Destination.Send");
                connectors.Add("Sams.Connector.MQTT.Destination.Publisher");
                connectors.Add("Sams.Connector.Odata.Destination.PostPayload");
                connectors.Add("Sams.Connector.OPC.Destination.Write(Customer Connector)");
                connectors.Add("Sams.Connector.Preactor.Destination.PCO");
                connectors.Add("Sams.Connector.Reporting.Destination.Docx");
                connectors.Add("Sams.Connector.Reporting.Destination.Rdlc");
                connectors.Add("Sams.Connector.SAP.Destination.RfcIdoc");
                connectors.Add("Sams.Connector.SAP.Destination.Rfc");
                connectors.Add("Sams.Connector.Soap.Destination.Http");
                connectors.Add("Sams.Connector.Void.Destination.StopProcess");
                connectors.Add("Sams.Connector.Void.Destination.Nop");
                connectors.Add("Sams.Connector.Wcf.Destination.Http");
                connectors.Add("Sams.Connector.Windows.Destination.StartProcess");
                break;
            default:
                // Handle unknown step
                connectors.Add("No connectors available for this step");
                break;
        }

        return Task.FromResult(connectors);
    }

    [KernelFunction]
    [Description("Creates a new connector object")]
    [return: Description("The created connector object")]
    public Task<IArtifact > CreateConnectorObject(
        //  [Description("The connector's parameters")] params object[] parameters
        [Description("The full name of the connector.")] string fullName, 
        [Description("A unique id for the connector, you can start at 1.")] string id,
        [Description("The description of the connector.")] string description,
        [Description("The Assembly Information of the connector.")] string assemblyInformation,
        [Description("The text in JSON format containing the information of the connector without the Configuration parameters that the user did not configure.")] string text
        )
    {   
        text = text.Trim();
        text = text.Replace("\n", " ").Replace("\r", "").Replace("\t", " "); 
        var connectorParser = new ConnectorParser();
        var (properties, attributes, configuration) = connectorParser.GenerateDictionaries(text);

        IArtifact connector = artifactFactory.CreateArtifact(
        fullName, id, description, assemblyInformation, properties, attributes, configuration);

        createdConnectors.Add(connector);

        return Task.FromResult(connector);
    }

    [KernelFunction]
    [Description("Returns all the already created connectors Configuration variables information")]
    [return: Description("A list of information about the already created connectors Configuration variables")]
    public Task<List<string>> GetCreatedConnectorsConfigurationVariables()
    {
        List<string> connectorsConfigurationVariables = new List<string>();

        foreach (var connector in createdConnectors)
        {
            connectorsConfigurationVariables.Add(connector.GetAllConfigurationVariables());
        }

        return Task.FromResult(connectorsConfigurationVariables);
    }

    [KernelFunction]
    [Description("Returns all the already created connectors information")]
    [return: Description("A list of information about the already created connectors")] 
    public Task<List<string>> GetCreatedConnectorsInformation()
    {
        List<string> connectorsInformation = new List<string>();

        foreach (var connector in createdConnectors)
        {
            connectorsInformation.Add(connector.GetAllInformation());
        }

        return Task.FromResult(connectorsInformation);
    }

    // [KernelFunction]
    // [Description("Changes a configuration parameter value of a connector")]
    // [return: Description("The updated connector object")]
    // public Task<IArtifact> ChangeConnectorConfiguration( 
    //     [Description("Id of the connector.")] int connectorId,
    //     [Description("The configuration parameters to change.")] string parameter,
    //     [Description("The new value of the configuration parameter.")] string newValue
    //     )
    // {
    //     var connector = createdConnectors.FirstOrDefault(c => c.Id == connectorId);
    //     if (connector != null)
    //     {
    //         if (connector.Configuration.ContainsKey(parameter))
    //         {
    //             connector.Configuration[parameter].Value = newValue;
    //         }
    //     }

    //     return Task.FromResult(connector ?? throw new ArgumentNullException(nameof(connector)));
    // }

    [KernelFunction]
    [Description("Deletes a configuration parameter of a connector that the user doesn't need")]
    [return: Description("The updated connector object")]
    public Task<IArtifact> DeleteConnectorConfiguration( 
        [Description("Id of the connector.")] int connectorId,
        [Description("The configuration parameters to delete.")] string parameter
        )
    {
        var connector = createdConnectors.FirstOrDefault(c => c.Id == connectorId);
        if (connector != null)
        {
            if (connector.Configuration.ContainsKey(parameter))
            {
                connector.Configuration.Remove(parameter);
            }
        }

        return Task.FromResult(connector ?? throw new ArgumentNullException(nameof(connector)));
    }

    [KernelFunction]
    [Description("Adds or changes a configuration parameter of a connector that the user needs")]
    [return: Description("The updated connector object")]
    public Task<IArtifact> AddConnectorConfiguration( 
        [Description("Id of the connector.")] int connectorId,
        [Description("The configuration parameters to add.")] string parameter,
        [Description("The description of the parameter to add.")] string parameterDescription,
        [Description("The value of the configuration parameter to add.")] string value
        )
    {
        var connector = createdConnectors.FirstOrDefault(c => c.Id == connectorId);
        if (connector != null)
        {
            if (!connector.Configuration.ContainsKey(parameter))
            {
                connector.Configuration.Add(parameter, new ConfigurationInfo { Description = parameterDescription, Value = value });
            } else {
                connector.Configuration[parameter].Value = value;
            }
        }
        return Task.FromResult(connector ?? throw new ArgumentNullException(nameof(connector)));
    }

    [KernelFunction]
    [Description("Returns the XML code of the connector to be inserted in the data broker tool.")]
    [return: Description("The XML code of the connector in string format.")]
    public async Task<FunctionResult> GetXmlConnector(
        [Description("The connector's Id.")] int Id,
        Kernel kernel
        )
    {
        var connector = createdConnectors.FirstOrDefault(c => c.Id == Id);
        var text = connector?.GetAllInformation();
        
        // Generate the XML file  
        var XmlGenerationFunctionPath = "C:/Users/z000p01m/Documents/Stage/code/RAG_System_V3/RecommandationSystem/plugins/Prompts"; 
        var xmlGenerationFunction = kernel.ImportPluginFromPromptDirectory(XmlGenerationFunctionPath);

        var arguments = new KernelArguments() { ["text"] = text };

        var result = await kernel.InvokeAsync(xmlGenerationFunction["XmlGeneration"], arguments);
        return result;
    }
}