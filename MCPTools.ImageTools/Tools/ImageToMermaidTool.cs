using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

namespace MCPTools.ImageTools.Tools;

/// <summary>
/// A tool for converting images to Mermaid diagrams using LLM vision capabilities.
/// </summary>
[McpServerToolType]
public sealed partial class ImageToMermaidTool
{
    private readonly IChatClient _chatClient;
    private readonly ILogger<ImageToMermaidTool> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageToMermaidTool"/> class.
    /// </summary>
    /// <param name="chatClient">The AI chat client.</param>
    /// <param name="logger">The logger instance.</param>
    public ImageToMermaidTool(IChatClient chatClient, ILogger<ImageToMermaidTool> logger)
    {
        _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Converts an image to a Mermaid diagram.
    /// </summary>
    /// <param name="imageData">Base64-encoded image data.</param>
    /// <param name="diagramType">Type of diagram to generate (flowchart, sequence, class, etc.).</param>
    /// <param name="additionalInstructions">Optional instructions to guide the diagram generation.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A string containing the Mermaid diagram code.</returns>
    [McpServerTool(Name = "ConvertImageToMermaid"), Description("Converts an image to a Mermaid diagram")]
    public async Task<string> ConvertImageToMermaid(
        [Description("Base64-encoded image data")] string imageData,
        [Description("Type of diagram to generate (flowchart, sequence, class, etc.)")] string diagramType = "flowchart",
        [Description("Optional instructions to guide the diagram generation")] string? additionalInstructions = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting image to Mermaid conversion, diagram type: {DiagramType}", diagramType);

        try
        {
            // Validate the image data
            if (string.IsNullOrEmpty(imageData))
            {
                throw new ArgumentException("Image data cannot be empty", nameof(imageData));
            }

            // Create the prompt for the AI
            var promptText = new StringBuilder();
            promptText.AppendLine("Convert the following image to a Mermaid diagram.");
            promptText.AppendLine($"Diagram type: {diagramType}");
            
            if (!string.IsNullOrEmpty(additionalInstructions))
            {
                promptText.AppendLine($"Additional instructions: {additionalInstructions}");
            }
            
            promptText.AppendLine("Please provide only the valid Mermaid diagram code, without any explanations or markdown formatting.");

            // Create a chat message collection
            var chatHistory = new List<ChatMessage>
            {
                // Add system message with instructions
                new (ChatRole.System, promptText.ToString())
            };
            
            // Create user message with the image
            var userMessage = new ChatMessage();
            
            // Add the image content as DataContent
            userMessage.Contents.Add(new DataContent(
                Convert.FromBase64String(imageData),
                "image/jpeg"));
            
            // Add the user message to the chat history
            chatHistory.Add(userMessage);

            var chatOptions = new ChatOptions()
            {
                Temperature = 0.2f, // Lower temperature for more deterministic output
                MaxOutputTokens = 2000    // Allow enough tokens for complex diagrams
            };

            // Send the request to the AI
            var response = await _chatClient.GetResponseAsync(chatHistory, chatOptions, cancellationToken);
            var assistantMessage = response.Text;
            
            // Extract and clean the Mermaid diagram code
            var diagramCode = ExtractMermaidCode(assistantMessage);
            
            _logger.LogInformation("Successfully generated Mermaid diagram");
            return diagramCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting image to Mermaid diagram");
            throw;
        }
    }

    /// <summary>
    /// Extracts clean Mermaid code from AI response text.
    /// </summary>
    /// <param name="responseText">The raw text from the AI response.</param>
    /// <returns>Cleaned Mermaid diagram code.</returns>
    private static string ExtractMermaidCode(string responseText)
    {
        // Look for code blocks in markdown format
        var codeBlockMatch = MermaidRegex().Match(responseText);

        return codeBlockMatch.Success 
            ? codeBlockMatch.Groups[1].Value.Trim() :
            responseText.Trim(); // If no code block is found, clean the entire response
    }

    [GeneratedRegex(@"```(?:mermaid)?\s*([\s\S]*?)```", RegexOptions.IgnoreCase, "en-DE")]
    private static partial System.Text.RegularExpressions.Regex MermaidRegex();
}