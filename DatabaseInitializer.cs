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
  
using System.Xml;

namespace Smart_Sams
{
    public class DatabaseInitializer
    {
        public static MemoryBuilder InitializeMemory(string endpoint, string apiKey)
        {
            var memoryBuilder = new MemoryBuilder();
            memoryBuilder.WithAzureOpenAITextEmbeddingGeneration(
                "text-embedding-ada-002",
                endpoint,
                apiKey,
                "model-id");

            // You can easily change the connector to db here
            var chromaMemoryStore = new ChromaMemoryStore("http://127.0.0.1:8000");
            memoryBuilder.WithMemoryStore(chromaMemoryStore);

            return memoryBuilder;
        }

        // public static void XMLParser(string path)
        // {
        //     string xmlContent = path;  

        //     XmlDocument xmlDoc = new XmlDocument();
        //     xmlDoc.LoadXml(xmlContent);

        //     // Extract information from Section[6]
        //     // Should be more then Section[6] in the future, while loop over 6<i<10
        //     XmlNode section6Node = xmlDoc.SelectSingleNode("/Document/Section[6]");
        //     if (section6Node != null)
        //     {
        //         // Extract connectors
        //         XmlNodeList connectorNodes = section6Node.SelectNodes("./Table");
        //         foreach (XmlNode connectorNode in connectorNodes)
        //         {
        //             // Extract connector name
        //             string connectorName = connectorNode.SelectSingleNode("./Row[1]/C0/p/text()").InnerText;
        //             Console.WriteLine($"Connector Name: {connectorName}");

        //             // Extract connector description
        //             string connectorDescription = connectorNode.SelectSingleNode("./Row[2]/C0/p/text()").InnerText;
        //             Console.WriteLine($"Connector Description: {connectorDescription}");

        //             // Extract information from Row[3]
        //             ExtractInformationFromRow(connectorNode.SelectSingleNode("./Row[3]"));

        //             // Extract information from Row[4]
        //             ExtractInformationFromRow(connectorNode.SelectSingleNode("./Row[4]"));
  
        //         }
        //     }
        // }

        // static void ExtractInformationFromRow(XmlNode rowNode)
        // {
        //     // Extract information from C0 and C1
        //     XmlNode c0Node = rowNode.SelectSingleNode("./C0");
        //     string name = c0Node.SelectSingleNode("./p/text()").InnerText;

        //     XmlNode c1Node = rowNode.SelectSingleNode("./C1");
        //     string defaultValue = c1Node.SelectSingleNode("./p/text()").InnerText;

        //     Console.WriteLine($"Name: {name}, Default Value: {defaultValue}");

        //     // Extract information from C2 (if present)
        //     XmlNode c2Node = rowNode.SelectSingleNode("./C2");
        //     if (c2Node != null)
        //     {
        //         string additionalInfo = c2Node.SelectSingleNode("./p/text()").InnerText;
        //         Console.WriteLine($"Additional Info: {additionalInfo}");
        //     }
        // }
    }
}
