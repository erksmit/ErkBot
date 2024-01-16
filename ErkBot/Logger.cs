using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace ErkBot;
public static class Logger
{
    public static DiscordChannel channel;

    public static event EventHandler<LogEventArgs>? LogEvent;

    public static void Log(LogLevel level, string message, object? sender = null)
    {
        var args = new LogEventArgs(message, level);
        LogEvent?.Invoke(sender, args);
        //TODO: log to console
    }
}
