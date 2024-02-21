using System.ComponentModel;
using Azure;
using Microsoft.SemanticKernel; 
using Microsoft.SemanticKernel.ChatCompletion;

using Microsoft.SemanticKernel.Plugins.Document;

namespace Plugins;
public class TransformationPlugin 
{   
    private readonly string inputPath = "../../input";
    private readonly string outputPath = "../../output";

    public TransformationPlugin(string inputPath, string outputPath)
    {
        this.inputPath = inputPath;
        this.outputPath = outputPath;
    }

    [KernelFunction]
    [Description("Helps the user to create a transformation file")]
    [return: Description("The transformation file created by the user")]
    public Task<string> MakeTransformationFile( 
        [Description("The information about the xsl file to generate")] string request
    )
    {    
        // Create a new file in the output directory
        string fileName = "transformation.xsl";
        string filePath = outputPath + "/" + fileName;
        File.WriteAllText(filePath, request);
        return Task.FromResult(filePath);
    }
}