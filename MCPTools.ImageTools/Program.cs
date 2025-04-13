var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMcpServer()
    .WithTools<ImageToMermaidTool>();

builder.Services.AddServiceDefaults();

var app = builder.Build();

app.MapMcp();

app.Run();
