using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MCPTools.CsTools.Tools;

public class UserCodeAssembly
{
  public ImmutableArray<Diagnostic> Diagnostics { get; set; }

  public UserCodeAssembly(string assemblyName, string userCode, List<string> references)
  {
    // Comile the user code using Roselyn
    var compilation = CSharpCompilation.Create(assemblyName);
    var syntaxTree = CSharpSyntaxTree.ParseText(userCode);
    var execRefs = references.Select(r => MetadataReference.CreateFromFile(r)).ToList();
    compilation = compilation.AddReferences(execRefs);
    compilation = compilation.AddSyntaxTrees(syntaxTree);
    var result = compilation.Emit(assemblyName);

    Diagnostics = result.Diagnostics;
  }
}
