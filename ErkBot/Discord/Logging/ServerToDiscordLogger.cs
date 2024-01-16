using DSharpPlus.Entities;
using ErkBot.Server;

namespace ErkBot.Discord.Logging;
internal class ServerToDiscordLogger
{
    private readonly BufferedDiscordChannel channel;

    private readonly BaseServer server;

    public ServerToDiscordLogger(BaseServer server, DiscordChannel channel)
    {
        this.server = server;
        this.channel = new BufferedDiscordChannel(channel);
    }

    public void Start()
    {
        channel.Start();
        server.OutputReceived += OnServerMessageReceived;
    }

    private void OnServerMessageReceived(object? sender, OutputReceivedEventArgs args)
    {
        var message = args.Message;
        channel.QueueMessage(message);
    }

}
