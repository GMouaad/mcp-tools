using DotNetIsolator;
using Microsoft.CodeAnalysis;

namespace MCPTools.CsTools.Tools;

public class UserCodeRunner
{
  private readonly IsolatedRuntimeHost _runtimeHost;
  private UserCodeAssembly _userCodeAssembly;

  public UserCodeRunner(IsolatedRuntimeHost runtimeHost)
  {
    _runtimeHost = runtimeHost;
  }

  public void Compile(string userCode, Action<string> StatusUpdateCallback,
    Action<CompilationResult> CompilationResultCallback = null)
  {
    StatusUpdateCallback.Invoke("Creating Sandbox Environment..");
    // Create a complete program from the user's code
    using var runtime = new IsolatedRuntime(_runtimeHost);

    StatusUpdateCallback.Invoke("Compiling Code..");
    var assemblyName = "UserCodeAssembly";
    var binDir = Path.GetDirectoryName(typeof(UserCodeRunner).Assembly.Location)!;
    var references = new List<string>
    {
      typeof(UserCodeRunner).Assembly.Location,
      Path.Combine(binDir, "IsolatedRuntimeHost", "WasmAssemblies", "System.Linq.dll", "DotNetIsolator.dll")
    };

    _userCodeAssembly = new UserCodeAssembly(assemblyName,
      $@"
      using System;
      using System.Collections.Generic;
      using System.Linq;

      namespace Sandbox;

      public class Runner
      {{
          public static void Run(string[] args)
          {{
              {userCode}
          }}
      }}
      ", references);

    if (_userCodeAssembly.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
    {
      // Handle compilation errors
      var errors = _userCodeAssembly.Diagnostics
        .Where(d => d.Severity == DiagnosticSeverity.Error)
        .Select(d => new CompilationError()
        {
          Id = d.Id,
          Message = d.GetMessage(),
          Severity = d.Severity.ToString(),
          Line = d.Location.GetLineSpan().StartLinePosition.Line,
        }).ToList();

      StatusUpdateCallback.Invoke($"Compilation Failed with {errors.Count} errors.");
      CompilationResultCallback?.Invoke(CompilationResult.Error(errors));
      return;
    }

    StatusUpdateCallback.Invoke("Compilation Succeeded");
    CompilationResultCallback?.Invoke(CompilationResult.Ok());

    var instance = runtime.CreateObject(assemblyName, "Sandbox", "Runner");

    // Execute the code
    StatusUpdateCallback.Invoke("Executing Code..");
    instance.InvokeVoid("Run");
  }
}
