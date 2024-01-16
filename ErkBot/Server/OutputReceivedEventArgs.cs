namespace ErkBot.Server;
public class OutputReceivedEventArgs(string message) : EventArgs
{
    public string Message { get; } = message;
}
