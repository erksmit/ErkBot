using Microsoft.Extensions.Logging;

namespace ErkBot;
public class LogEventArgs : EventArgs
{
    public LogEventArgs(string message, LogLevel level)
    {
        Message = message;
        Level = level;
    }

    public string Message { get; init; }

    public LogLevel Level { get; init; }
}
