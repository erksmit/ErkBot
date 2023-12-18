using DSharpPlus;
using ErkBot.Server.Configuration;

namespace ErkBot.Server.Types;
public class MinecraftServer(DiscordClient client, ExecutableServerConfiguration config) : ExecutableServer(client, config)
{
}
