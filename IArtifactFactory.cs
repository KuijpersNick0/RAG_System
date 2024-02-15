namespace classes
{
    public interface IArtifactFactory
    {
        // IArtifact CreateArtifact(string fullName, string id, string description, string assemblyInformation, Dictionary<string, string> properties, Dictionary<string, string> attributes, Dictionary<string, string> configuration);
        // This method below is more robust and flexible
        IArtifact CreateArtifact(params object[] parameters);
    }
}
