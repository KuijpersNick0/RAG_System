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
    public Dictionary<string, AttributeInfo> Attributes { get; set; }
    public Dictionary<string, ConfigurationInfo> Configuration { get; set; }

    public EventListener(string fullName, int id, string description, string assemblyInformation)
    {
        FullName = fullName;
        Id = id;
        Description = description;
        AssemblyInformations = assemblyInformation;
        Properties = Properties ?? new Dictionary<string, string>();
        Attributes = Attributes ?? new Dictionary<string, AttributeInfo>();
        Configuration = Configuration ?? new Dictionary<string, ConfigurationInfo>();
    }

    public string GetAllConfigurationVariables()
    {
        return Configuration != null ? string.Join(", ", Configuration.Select(kv => $"{kv.Key}: {kv.Value.Description}={kv.Value.Value}")) : "None";
    }

    public string GetAllInformation()
    {
        var propertiesString = Properties != null ? string.Join(", ", Properties.Select(kv => $"{kv.Key}: {kv.Value}")) : "None";
        var attributesString = Attributes != null ? string.Join(", ", Attributes.Select(kv => $"{kv.Key}: {kv.Value.Description}={kv.Value.Value}")) : "None";
        var configurationString = Configuration != null ? string.Join(", ", Configuration.Select(kv => $"{kv.Key}: {kv.Value.Description}={kv.Value.Value}")) : "None";

        return $"FullName: {FullName}, Id: {Id}, Description: {Description}, AssemblyInformations: {AssemblyInformations}, Properties: {propertiesString}, Attributes: {attributesString}, Configuration: {configurationString}";
    }
}

public class AttributeInfo
{
    public string? Description { get; set; }
    public string? Value { get; set; }
}

public class ConfigurationInfo
{
    public string? Description { get; set; }
    public string? Value { get; set; }
}
