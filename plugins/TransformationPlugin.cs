using System.ComponentModel;
using Azure;
using Microsoft.SemanticKernel; 
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.Memory;

namespace Plugins;
public class TransformationPlugin 
{   
    private readonly string inputPath = "../../input";
    private readonly string outputPath = "../../output";    
    private readonly string transformationPath = "../../transformationFiles";
    private readonly ISemanticTextMemory memory;

    public TransformationPlugin(string inputPath, string outputPath)
    {
        this.inputPath = inputPath;
        this.outputPath = outputPath;
        // Creating the memory to store files => Can be done in another file later on and pass on memory as a parameter to plugin
        var memoryBuilder = new MemoryBuilder();
        memoryBuilder.WithMemoryStore(new VolatileMemoryStore()); 
        var memory = memoryBuilder.Build();  
        this.memory = memory;
    }

    private void StoreInputOutput()
    { 
        const string InputMemoryCollection = "InputMemory";
        const string OutputMemoryCollection = "OutputMemory";

        // Read all files inside the inputPath folder (more then 1 exemple ?)
        string[] inputFiles = Directory.GetFiles(inputPath);
        foreach (string inputFile in inputFiles)
        {
            // Storing each input file in the memory
            memory.SaveReferenceAsync(
                collection: InputMemoryCollection,
                description: "Input file that needs to be transformed",
                text: File.ReadAllText(inputFile),
                externalId: Guid.NewGuid().ToString(),
                externalSourceName: Path.GetFileName(inputFile)
            );  
        }

        // Read all files inside the outputPath folder
        string[] outputFiles = Directory.GetFiles(outputPath);
        foreach (string outputFile in outputFiles)
        {
            // Storing each output file in the memory
            memory.SaveReferenceAsync(
                collection: OutputMemoryCollection,
                description: "Output file that has been transformed",
                text: File.ReadAllText(outputFile),
                externalId: Guid.NewGuid().ToString(),
                externalSourceName: Path.GetFileName(outputFile)
            );
        }
    }

    [KernelFunction]
    [Description("Read from collection memory")]
    [return: Description("The content of the file")]
    public async Task<string> ReadFromMemory(
        Kernel kernel,
        [Description("The name of the collection to read from")] string collectionName,
        [Description("The id of the file to read")] string fileId
    )
    { 
        var response = await memory.GetAsync(collectionName, fileId, true, kernel);
        string result = string.Empty; 
        if (response != null)
        { 
            result = response.Metadata.Text;
        }
        return result;
    }

    [KernelFunction]
    [Description("Helps the user to create a transformation file")]
    [return: Description("The transformation file created by the user")]
    public void MakeTransformationFile( 
        [Description("The information about the xsl file to generate")] string request
    )
    {      
        // Start by creating volatile memory with input and output files
        // Check input and output folder are not empty
        if (Directory.GetFiles(inputPath).Length == 0 || Directory.GetFiles(outputPath).Length == 0)
        {
            throw new Exception("Input and output folders should not be empty");
        } else {
            StoreInputOutput();
        }
 
        // Create a new file in the tranformationFiles directory  
        string filePath = transformationPath + "/" + Guid.NewGuid().ToString() + ".xsl";
        // Handle logic here to create the transformation file
        File.WriteAllText(filePath, request); 
    }
}