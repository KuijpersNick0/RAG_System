namespace classes
{
    public interface IArtifactFactory
    {
        IArtifact CreateArtifact(string fullName, string id, string description, string assemblyInformation, Dictionary<string, string> properties, Dictionary<string, AttributeInfo> attributes, Dictionary<string, ConfigurationInfo> configuration);
        // This method below is more robust and flexible but doesnt work with SK
        // IArtifact CreateArtifact(params object[] parameters);
    }
}
