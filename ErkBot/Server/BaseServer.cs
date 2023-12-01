using DSharpPlus;
using DSharpPlus.Entities;

namespace ErkBot.Server;
public abstract class BaseServer
{
    public string DisplayName { get; }
    public DiscordChannel LogChannel { get; protected set; }

    public bool Enabled { get; }

    protected readonly DiscordClient discordClient;
    protected readonly ulong logChannelId;

    public BaseServer(DiscordClient client, ServerConfiguration config)
    {
        discordClient = client;
        DisplayName = config.Name;
        Enabled = config.Enabled;
        logChannelId = config.OutputChannelId;
    }

    public abstract event EventHandler<ServerMessageReceivedEventArgs>? MessageReceived;

    public ServerStatus Status { get; protected set; }

    public abstract Task<bool> Start();

    public abstract Task<bool> Stop(int timeOut);

    public abstract bool Kill();
}
