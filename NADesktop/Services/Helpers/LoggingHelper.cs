
using NADesktop.Models.Enums;
using System.Net;
using System.Reflection.Emit;
using System.Text.Json.Serialization;

namespace NADesktop.Services.Helper;

public class LoggingHelper
{
    [JsonPropertyName("type")]
    public TypeMessage Type { get; set; } = TypeMessage.Problem;
    [JsonPropertyName("dangerLevel")]
    public DangerLevel DangerLevel { get; set; } = DangerLevel.Error;
    [JsonPropertyName("message")]
    public string Message { get; set; } = "Unprocessed Error";
    [JsonPropertyName("description")]
    public string? Description { get; set; } = string.Empty;
    public HttpStatusCode? StatusCode { get; set; }

    public LoggingHelper() { }

    public LoggingHelper(TypeMessage typeLevel, DangerLevel dangerLevel, string messange, string? description = null)
    {
        Type = typeLevel;
        DangerLevel = dangerLevel;
        Message = messange;
        Description = description;
    }
}
