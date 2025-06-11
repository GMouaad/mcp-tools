using System.ComponentModel;
using System.Text;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace MCPTools.OrchestratorTools;

[McpServerToolType]
public class WorkflowTools
{
  
  private readonly IChatClient _chatClient;
  private readonly ILogger<WorkflowTools> _logger;
  
  public WorkflowTools(IChatClient chatClient, ILogger<WorkflowTools> logger)
  {
    _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }
  
  [McpServerTool(Name = "WorkflowToMermaid")]
  [Description("Converts a workflow to a Mermaid diagram")]
  public async Task<string> ConvertWorkflowToMermaid(
    [Description("Workflow JSON data")] string workflowJson,
    [Description("Optional instructions to guide the diagram generation")] string? additionalInstructions = null,
    CancellationToken cancellationToken = default)
  {
    _logger.LogInformation("Starting workflow to Mermaid conversion");

    try
    {
      ArgumentException.ThrowIfNullOrEmpty(workflowJson);

      // Create the prompt for the AI
      var promptText = new StringBuilder();
      promptText.AppendLine("I have a workflow definition that is composted of so called activities which are the tasks the will be executed in this workflow. These activities are tied to each other generally using the NextActivities property that has an id and a name of the next activity. Can you generate a mermaid diagram of the workflow the shows the relationsships between the activities and the execution flow.");
      promptText.AppendLine(workflowJson);
      
      if (!string.IsNullOrEmpty(additionalInstructions))
      {
        promptText.AppendLine($"Additional instructions: {additionalInstructions}");
      }

      // Call the AI chat client with the prompt
      var response = await _chatClient.GetResponseAsync(promptText.ToString(), cancellationToken: cancellationToken);
      
      return response.Text;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error converting workflow to Mermaid");
      throw;
    }
  }
  
}