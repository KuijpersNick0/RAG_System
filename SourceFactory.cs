namespace classes
{
    public class SourceFactory : IArtifactFactory
    {
        public IArtifact CreateArtifact(params object[] parameters)
        {
            if (parameters.Length != 2){
                throw new ArgumentException("Incorrect number of parameters for SourceListener creation");
            }
            return new SourceListener
            {
                FullName = (string)parameters[0],
                Id = int.Parse((string)parameters[1]) 
            };
        }
    }
}