namespace ErkBot.Server;
public enum ServerStatus
{
    Stopped,
    Running,
    Crashed
}

public static class ServerStatusExtensions
{
    public static string? AsString(this ServerStatus status)
    {
        return Enum.GetName(typeof(ServerStatus), status);
    }
}