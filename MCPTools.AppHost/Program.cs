var builder = DistributedApplication.CreateBuilder(args);


var ollama = builder.AddOllama("ollama")
    .WithImageTag("0.6.5")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithOpenWebUI()
    .WithContainerRuntimeArgs("--gpus", "all")
  // .WithGPUSupport()
  ;

var gemma = ollama.AddModel("gemma3:4b");

var imageTools = builder.AddProject<Projects.MCPTools_ImageTools>("image-tools")
  .WithReference(gemma)
  .WithOtlpExporter();

// Run MCP Inspector
// as executable 
// npx @modelcontextprotocol/inspector
builder.AddExecutable("mcp-inspector-exec", "npx", "@modelcontextprotocol/inspector")
  .WithEnvironment("CLIENT_PORT", "5177")
  .WithHttpEndpoint(targetPort:5177)
  .WithExternalHttpEndpoints();

// inside docker
builder.AddContainer("mcp-inspector", "mcp/inspector")
  .WithReference(imageTools)
  .WithEnvironment("CLIENT_PORT", "5173")
  .WithHttpEndpoint(targetPort:5173)
  .WithExternalHttpEndpoints()
  .WithLifetime(ContainerLifetime.Persistent);

builder.Build().Run();
