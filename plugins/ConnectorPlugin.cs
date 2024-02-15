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
    [return: Description("The list of available connectors")]
    public Task<List<string>> GetConnectorsStep(
        [Description("The process step at which the user is currently for configuring his data broker tool.")] string step)
    {
        List<string> connectors = new List<string>();

        // Add connectors based on the current step
        switch (step)
        {
            case "EventListener":
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
    [return: Description("The list of created connector objects so far in the configuration process of the data broker tool")]
    public Task<List<IArtifact>> CreateConnectorsObject(
         [Description("The connector's parameters, ")] params object[] parameters
        )
    {
        // Log or debug the parameters to understand what is being passed
        Console.WriteLine($"Parameters: {string.Join(", ", parameters)}");
        
        IArtifact artifact = artifactFactory.CreateArtifact(parameters);
 
        createdConnectors.Add(artifact);

        return Task.FromResult(createdConnectors);
    }

    [KernelFunction]
    [Description("Returns all the already created connectors configuration variables information")]
    [return: Description("A list of information about the already created connectors configuration variables")]
    public Task<List<string>> GetCreatedConnectorsConfigurationVariables()
    {
        List<string> connectorsConfigurationVariables = new List<string>();

        foreach (var connector in createdConnectors)
        {
            connectorsConfigurationVariables.Add(string.Join(", ", connector.GetAllConfigurationVariables()));
        }

        return Task.FromResult(connectorsConfigurationVariables);
    }
}