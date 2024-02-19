namespace classes
{
    public class EventListenerFactory : IArtifactFactory
    {
        public IArtifact CreateArtifact(string fullName, string id, string description, string assemblyInformation, Dictionary<string, string> properties, Dictionary<string, AttributeInfo> attributes, Dictionary<string, ConfigurationInfo> configuration)
        {
            return new EventListener(fullName, int.Parse(id), description, assemblyInformation)
            { 
                Properties = properties ?? new Dictionary<string, string>(),
                Attributes = attributes ?? new Dictionary<string, AttributeInfo>(),
                Configuration = configuration ?? new Dictionary<string, ConfigurationInfo>()
            };
        }
    }
}
