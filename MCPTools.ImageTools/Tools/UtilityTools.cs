using System.ComponentModel;
using ModelContextProtocol.Server;

namespace MCPTools.ImageTools.Tools;


[McpServerToolType]
public class UtilityTools
{
  [McpServerTool(Name = "Echo"), Description("Echoes a message back to the client")]
  public static string Echo([Description("The message to echo back")] string message) => $"Hello {message}";
  
  // Get local time
  [McpServerTool(Name = "GetLocalTime"), Description("Gets the local time of the server")]
  public static DateTime GetLocalTime([Description("The time zone to get the local time for")] string timeZone = "UTC")
  {
    var localTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZone));
    return localTime;
  }
  
  
}