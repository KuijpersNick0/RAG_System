using System;
using Newtonsoft.Json;

namespace classes 
{ 
    public class ConnectorParser
    {

        public EventListener ParseConnectorInfo(string text)
        { 
            // Parse JSON text into EventListener object 
            var eventListener = JsonConvert.DeserializeObject<EventListener>(text);
            return eventListener ?? throw new Exception("Failed to deserialize JSON into EventListener object.");
        }

        public (Dictionary<string, string>, Dictionary<string, AttributeInfo>, Dictionary<string, ConfigurationInfo>) GenerateDictionaries(string text)
        {
            var connectorInfo = ParseConnectorInfo(text);

            return (
                connectorInfo.Properties ?? new Dictionary<string, string>(),
                connectorInfo.Attributes ?? new Dictionary<string, AttributeInfo>(),
                connectorInfo.Configuration ?? new Dictionary<string, ConfigurationInfo>()
            );
        }
    }
}
 