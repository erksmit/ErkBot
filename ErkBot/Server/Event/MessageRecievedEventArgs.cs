namespace ErkBot.Server.Event;
public class MessageRecievedEventArgs(string message) : EventArgs
{
    public string Message { get; } = message;
}
