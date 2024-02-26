using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace classes 
{ 
    public class ConnectorParser
    { 
        private JObject ParseConnectorInfo(string text)
        { 
            // Parse JSON text into JObject, to be handled into Connector object afterwards
            var connector = JsonConvert.DeserializeObject<JObject>(text);
            return connector ?? throw new Exception("Failed to deserialize JSON into Connector object.");
        }

        public (Dictionary<string, string>, Dictionary<string, AttributeInfo>, Dictionary<string, ConfigurationInfo>) GenerateDictionaries(string text)
        {
            var connectorInfo = ParseConnectorInfo(text);
 
            var properties = connectorInfo["Properties"]?.ToObject<Dictionary<string, string>>() ?? new Dictionary<string, string>();
            var attributes = connectorInfo["Attributes"]?.ToObject<Dictionary<string, AttributeInfo>>() ?? new Dictionary<string, AttributeInfo>();
            var configuration = connectorInfo["Configuration"]?.ToObject<Dictionary<string, ConfigurationInfo>>() ?? new Dictionary<string, ConfigurationInfo>();
            
            return (properties, attributes, configuration);
        }
    }
}
 