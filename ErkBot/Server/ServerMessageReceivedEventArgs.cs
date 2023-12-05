namespace ErkBot.Server;
public class ServerMessageReceivedEventArgs(string message) : EventArgs
{
    public string Message { get; } = message;
}
