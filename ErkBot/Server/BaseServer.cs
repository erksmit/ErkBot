using DSharpPlus;
using ErkBot.Server.Configuration;

namespace ErkBot.Server;
public abstract class BaseServer
{
    public BaseServer(DiscordClient client, IServerConfiguration config)
    {
        this.client = client;
        DisplayName = config.Name;
        Enabled = config.Enabled;
        logChannelId = config.OutputChannelId;
        Status = Enabled ? ServerStatus.Stopped : ServerStatus.Disabled;
        MessageReceived += SendReceivedMessageToDiscord;
    }

    public string DisplayName { get; }
    public bool Enabled { get; }
    public ServerStatus Status { get; protected set; }

    protected BufferedDiscordChannel? logChannel;
    private ulong logChannelId;
    protected DiscordClient client;

    public async virtual Task Start()
    {
        if (logChannel == null)
        {
            var channel = await client.GetChannelAsync(logChannelId);
            logChannel = new BufferedDiscordChannel(channel);
            logChannel.Start();
        }
    }

    public abstract Task Stop(int timeOut = 10_000);

    public event EventHandler<ServerMessageReceivedEventArgs> MessageReceived;

    protected void OnMessageReceived(object? sender, ServerMessageReceivedEventArgs args)
    {
        MessageReceived?.Invoke(sender, args);
    }

    private void SendReceivedMessageToDiscord(object? sender, ServerMessageReceivedEventArgs args)
    {
        logChannel?.QueueMessage(args.Message);
    }
}
