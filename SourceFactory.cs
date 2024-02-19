namespace classes
{
    public class SourceFactory : IArtifactFactory
    {
        public IArtifact CreateArtifact(string fullName, string id, string description, string assemblyInformation, Dictionary<string, string> properties, Dictionary<string, AttributeInfo> attributes, Dictionary<string, ConfigurationInfo> configuration)
        {
            return new Source
            {
                FullName = fullName,
                Id = int.Parse(id), 
                Description = description,
                AssemblyInformations = assemblyInformation,
                Properties = properties ?? new Dictionary<string, string>(),
                Attributes = attributes ?? new Dictionary<string, AttributeInfo>(),
                Configuration = configuration ?? new Dictionary<string, ConfigurationInfo>()
            };
        }
    }
}