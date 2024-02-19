namespace classes
{
    public class Source : IArtifact
    {
        public string FullName { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public string AssemblyInformations { get; set; }
        public Dictionary<string, string> Properties { get; set; }
        public Dictionary<string, AttributeInfo> Attributes { get; set; }
        public Dictionary<string, ConfigurationInfo> Configuration { get; set; }

        public Source()
        {
            throw new NotImplementedException();
        }
    }
}