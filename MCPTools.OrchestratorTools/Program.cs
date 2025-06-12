using System.ComponentModel.DataAnnotations;
using MCPTools.OrchestratorTools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));

builder.Services.AddScoped<WorkflowTools>();

// Add the Ollama chat client using aspire client integration
builder.AddOllamaApiClient("ollama-gemma3")
  .AddChatClient()
  .UseDistributedCache()
  .UseOpenTelemetry()
  .UseLogging();

builder.Services.AddMcpServer()
  .WithHttpTransport()
  .WithPromptsFromAssembly()
  .WithToolsFromAssembly();

builder.AddServiceDefaults();

builder.Services.ConfigureHttpClientDefaults(http =>
{
#pragma warning disable EXTEXP0001
  http.RemoveAllResilienceHandlers();
#pragma warning restore EXTEXP0001
  http.ConfigureHttpClient(o => o.Timeout = TimeSpan.FromMinutes(3));
  
  // Turn on service discovery by default
  http.AddServiceDiscovery();
});

var app = builder.Build();

app.MapMcp();


app.Run();