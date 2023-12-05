using System.Diagnostics;

using DSharpPlus;

namespace ErkBot.Server;
public class MinecraftServer(DiscordClient client, ServerConfiguration config) : BaseServer(client, config)
{
    protected override void ServerOutputReceived(object sender, DataReceivedEventArgs message)
    {
        if (message.Data != null)
        {
            OnMessageReceived(this, new ServerMessageReceivedEventArgs(message.Data));
        }
    }
}
