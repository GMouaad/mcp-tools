using DotNetIsolator;
using MCPTools.CsTools.Tools;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));

// Add the Ollama chat client using aspire client integration
builder.AddOllamaApiClient("ollama-gemma3")
  .AddChatClient()
  .UseDistributedCache()
  .UseOpenTelemetry()
  .UseLogging();

builder.Services.AddMcpServer()
  .WithHttpTransport()
  .WithPromptsFromAssembly()
  .WithTools<RoslynTools>()
  .WithTools<SandboxTools>();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();
