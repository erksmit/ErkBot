using DSharpPlus;
using DSharpPlus.Entities;
using ErkBot.Server.Event;

namespace ErkBot.Server;
public abstract class BaseServer
{
    public string DisplayName { get; }
    public DiscordChannel LogChannel { get; }

    public bool Enabled { get; }

    public BaseServer(DiscordChannel logChannel, ServerConfiguration config)
    {
        LogChannel = logChannel;
        DisplayName = config.Name;
        Enabled = config.Enabled;
    }

    public event EventHandler<MessageRecievedEventArgs>? MessageRecieved;

    public ServerStatus Status { get; protected set; }

    public abstract Task Start(double timeOut);

    public abstract Task Stop(double timeOut);

    public abstract void Kill();
}
