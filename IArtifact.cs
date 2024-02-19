using System;
using System.Collections.Generic;

namespace classes;
public interface IArtifact
{
    string FullName { get; set; }
    int Id { get; set; }
    string Description { get; set; }
    string AssemblyInformations { get; set; }
    Dictionary<string, string> Properties { get; set; }
    Dictionary<string, AttributeInfo> Attributes { get; set; }
    Dictionary<string, ConfigurationInfo> Configuration { get; set; }


    // Overridable interface methods :/ following Paul's code (I think) otherwise abstract class would be better
    public string GetAllConfigurationVariables()
    { 
        return "None";
    }

    public string GetAllInformation()
    { 
        return "FullName: " + FullName + ", Id: " + Id;
    }
}
