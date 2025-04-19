using System.ComponentModel;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using ModelContextProtocol.Server;

namespace MCPTools.CsTools.Tools;

public class RoslynTools
{
    // Default using directives commonly needed
    private static readonly List<string> DefaultUsings =
    [
        "System",
        "System.Collections.Generic",
        "System.Linq",
        "System.Text",
        "System.Threading.Tasks",
        "System.Text.Json"
    ];
    
    [McpServerTool(Name = "CompileCSharpClass"), Description("Compiles a C# class and returns errors if any")]
    public static CompilationResult CompileCSharpClass(
        [Description("The C# code to compile")] string code,
        [Description("The using directives to include in the compilation")] List<string>? usings = null)
    {
        
        
        // Combine default and custom usings
        var allUsings = DefaultUsings.ToList();
        if (usings != null)
        {
            allUsings.AddRange(usings);
        }
        
        // Create the full source code with using directives
        var sourceCode = string.Join(Environment.NewLine, allUsings.Select(u => $"using {u};")) +
                         Environment.NewLine + Environment.NewLine +
                         code;
        
        // Parse the source code
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        
        // Reference assemblies needed for compilation
        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location)
        };
        
        // Create compilation
        var compilation = CSharpCompilation.Create(
            "DynamicCompilation",
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        
        // Compile and get diagnostics
        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);
        
        if (result.Success)
        {
            return CompilationResult.Ok();
        }

        // Format errors into structured JSON
        var errors = result.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .Select(d => new CompilationError()
            {
                Id = d.Id,
                Line = d.Location.GetLineSpan().StartLinePosition.Line + 1,
                Column = d.Location.GetLineSpan().StartLinePosition.Character + 1,
                Message = d.GetMessage(),
                Severity = d.Severity.ToString()
            })
            .ToList();
            
        return CompilationResult.Error(errors);
    }
    
}

/// <summary>
/// Represents a compilation error with detailed information.
/// </summary>
public class CompilationResult
{
    public bool Success { get; set; }

    public CompilationError[] Errors { get; set; } = [];
    public static CompilationResult Ok() => new()
    {
        Success = true,
    };
    
    public static CompilationResult Error(IEnumerable<CompilationError> errors) => new()
    {
        Success = false,
        Errors = errors.ToArray()
    };
}

public class CompilationError
{
    public string Id { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }
    public string Message { get; set; }
    public string Severity { get; set; }
}