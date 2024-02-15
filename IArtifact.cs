using System;
using System.Collections.Generic;

namespace classes;
public interface IArtifact
{
    string FullName { get; set; }
    int Id { get; set; }

    public Dictionary<string, string> GetAllConfigurationVariables()
    { 
        return new Dictionary<string, string>();
    }
}
