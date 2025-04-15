using System.ComponentModel.DataAnnotations;
using MCPTools.ImageTools.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDistributedMemoryCache();
builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));

builder.Services.AddScoped<ImageToMermaidTool>();

// Add the Ollama chat client using aspire client integration
builder.AddOllamaApiClient("ollama-gemma3")
  .AddChatClient()
  .UseLogging()
  .UseOpenTelemetry();

builder.Services.AddMcpServer()
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

app.MapMcp(""
//   , runSessionAsync: (context, server, ct) =>
// {
//   server.RegisterNotificationHandler("Echo", (notification, token) =>
//   {
//     var message = notification.Params?["message"]?.GetValue<string>();
//     context.RequestServices.GetService<ILogger>()?.LogInformation("Received message: {Message}", message);
//     // return Task.FromResult($"Hello {message}");
//     return Task.CompletedTask;
//   });
//   return Task.CompletedTask;}
);

// POST API to convert image to mermaid
app.MapPost("/convert-image-to-mermaid", async (
    [FromServices] ILogger<ImageToMermaidTool> logger,
    [FromServices] ImageToMermaidTool tool,
    [FromForm][Required] IFormFile imageFile,
    [FromQuery] string? diagramType,
    [FromQuery] string? additionalInstructions, CancellationToken cancellationToken) =>
  {
    // --- Security: Validate File Type (Content Type and/or Extension) ---
    // string[] allowedContentTypes = ["image/jpeg", "image/png", "image/gif", "image/bmp"];
    string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp"];
    var fileExtension = Path.GetExtension(imageFile.FileName)?.ToLowerInvariant();

    if (string.IsNullOrEmpty(fileExtension) ||
        !allowedExtensions.Contains(fileExtension) 
        // || !allowedContentTypes.Contains(imageFile.ContentType.ToLowerInvariant())
        )
    {
      logger.LogWarning("Upload attempt with invalid file type: {FileName} ({ContentType})",
        imageFile.FileName, imageFile.ContentType);
      return Results.BadRequest(new { Message = "Invalid file type. Allowed types: JPG, PNG, GIF, BMP." });
    }

    // --- Security: Consider File Size Limits ---
    // Defined by server config (Kestrel limits) or attributes like [RequestSizeLimit]
    // Add a manual check if needed:
    const long maxFileSize = 100 * 1024 * 1024; // Example: 100 MB limit
    if (imageFile.Length > maxFileSize)
    {
      logger.LogWarning("Upload attempt with file exceeding size limit: {FileName} ({Size} bytes)",
        imageFile.FileName, imageFile.Length);
      return Results.BadRequest(new
        { Message = $"File exceeds maximum size limit of {maxFileSize / 1024 / 1024} MB." });
    }

    // file stream to base64
    byte[] imageBytes;
    try
    {
      // --- Read file content into memory ---
      // Be cautious with large files, this reads the entire file into RAM.
      await using var memoryStream = new MemoryStream();
      await imageFile.CopyToAsync(memoryStream, cancellationToken);
      imageBytes = memoryStream.ToArray();
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Error reading image file stream for {FileName}", imageFile.FileName);
      return Results.Problem("Error reading uploaded file.", statusCode: 500);
    }

    // --- Convert image bytes to Base64 string ---
    var base64ImageString = Convert.ToBase64String(imageBytes);
    var result =
      await tool.ConvertImageToMermaid(base64ImageString, diagramType, additionalInstructions, cancellationToken);
    return Results.Ok(result);
  }).WithName("UploadImage") // Optional: Name for OpenAPI/Swagger
  .Produces<string>(StatusCodes.Status201Created) // For Swagger: Success response type
  .Produces<string>(StatusCodes.Status400BadRequest) // For Swagger: Bad request response type
  .Produces<string>(StatusCodes.Status500InternalServerError) // For Swagger: Server error type
  .DisableAntiforgery();

app.Run();