
using NetworkAnalytics.Models.Enums;

namespace NetworkAnalytics.Services.Helper;

public class LogginHelper
{
    public TypeMessage Type { get; set; } = TypeMessage.Ordinary;
    public DangerLevel DangerLevel { get; set; } = DangerLevel.Oke;
    public string Message { get; set; } = "Okey";
    public string? Description { get; set; } = string.Empty;
    internal object? SomeObject { get; set; }

    public LogginHelper() { }

    public LogginHelper(TypeMessage typeLevel, DangerLevel dangerLevel, string messange, object? someObject = null)
    {
        Type = typeLevel;
        DangerLevel = dangerLevel;
        Message = messange;
        SomeObject = someObject;
    }

    public LogginHelper(LogginHelper logginMain)
    {
        Type = logginMain.Type;
        DangerLevel = logginMain.DangerLevel;
        Message = logginMain.Message;
        Description = logginMain.Description;
        SomeObject = logginMain.SomeObject;
    }

    public LogginHelper Clone()
    {
        return new LogginHelper(Type, DangerLevel, Message, SomeObject)
        {
            Description = Description
        };
    }
}
