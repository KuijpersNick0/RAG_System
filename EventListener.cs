using System;
using System.Collections.Generic;

namespace classes;

public class EventListener: IArtifact
{
    public string FullName { get; set; }
    public int Id { get; set; } 
    public string Description { get; set; }
    public string AssemblyInformations { get; set; }
    public Dictionary<string, string> Properties { get; set; }
    public Dictionary<string, string> Attributes { get; set; }
    public Dictionary<string, string> Configuration { get; set; }

    public EventListener()
    {
        Properties = new Dictionary<string, string>();
        Attributes = new Dictionary<string, string>();
        Configuration = new Dictionary<string, string>();
    }

    public Dictionary<string, string> GetAllConfigurationVariables()
    {
        Dictionary<string, string> configurationVariables = new Dictionary<string, string>();

        foreach (var config in Configuration)
        {
            configurationVariables.Add(config.Key, config.Value);
        }

        return configurationVariables;
    }
}
