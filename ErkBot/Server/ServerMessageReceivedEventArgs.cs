namespace ErkBot.Server;
public abstract class ServerMessageReceivedEventArgs(string message) : EventArgs
{
    public string Message { get; } = message;
}
