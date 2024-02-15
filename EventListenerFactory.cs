namespace classes
{
    public class EventListenerFactory : IArtifactFactory
    {
        public IArtifact CreateArtifact(params object[] parameters)
        {
            return new EventListener
            {
                FullName = (string)parameters[0],
                Id = int.Parse((string)parameters[1]),  
                Description = (string)parameters[2],
                AssemblyInformations = (string)parameters[3],
                Properties = (Dictionary<string, string>)parameters[4] ?? new Dictionary<string, string>(),
                Attributes = (Dictionary<string, string>)parameters[5] ?? new Dictionary<string, string>(),
                Configuration = (Dictionary<string, string>)parameters[6] ?? new Dictionary<string, string>()
            };
        }
    }
}
