using System.ComponentModel;
using System.Text;
using DotNetIsolator;
using ModelContextProtocol.Server;
using Wasmtime;

namespace MCPTools.CsTools.Tools;

/// <summary>
/// A tool for executing C# code in a secure sandbox environment.
/// </summary>
[McpServerToolType]
public class SandboxTools
{
  private readonly ILogger<SandboxTools> _logger;
  private readonly IsolatedRuntimeHost _runtimeHost;


  /// <summary>
  /// Initializes a new instance of the <see cref="SandboxTools"/> class.
  /// </summary>
  /// <param name="logger">The logger instance.</param>
  public SandboxTools(ILogger<SandboxTools> logger)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    var wasiConfig = new WasiConfiguration()
        .WithPreopenedDirectory(".", ".")
      // .WithEnvironmentVariables([])
      ;
    _runtimeHost = new IsolatedRuntimeHost()
      .WithBinDirectoryAssemblyLoader()
      .WithWasiConfiguration(wasiConfig);
    _logger.LogInformation("CSharp sandbox tool initialized with isolation host");
  }

  /// <summary>
  /// Executes C# code in a secure sandbox environment.
  /// </summary>
  /// <param name="code">The C# code to execute.</param>
  /// <param name="timeout">Optional timeout in milliseconds.</param>
  /// <returns>The execution result.</returns>
  [McpServerTool(Name = "ExecuteCSharp"), Description("Executes C# code in a secure sandbox environment")]
  public Task<CompilationResult> ExecuteCSharpAsync( // RODO: return ExecutionResult encapsulating result and errors
    [Description("C# code to execute")] string code,
    [Description("Timeout in milliseconds (default: 10000)")]
    int timeout = 10000)
  {
    _logger.LogInformation("Executing C# code in sandbox");

    CompilationResult result = null;
    var runner = new UserCodeRunner(_runtimeHost);
    runner.Compile(code,
      s => _logger.LogInformation("Compilation status: {Status}", s),
      r =>
      {
        _logger.LogError("Compilation error: {Error}", result);
        result = r;
      });
    return Task.FromResult(result);
  }
}
