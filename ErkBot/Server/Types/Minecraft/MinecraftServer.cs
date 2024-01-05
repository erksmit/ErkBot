using DSharpPlus;
using ErkBot.Server.Configuration;
using ErkBot.Server.Types.Minecraft.Model;

namespace ErkBot.Server.Types.Minecraft;
public class MinecraftServer : ExecutableServer
{
    private readonly MinecraftServerConfiguration config;
    public MinecraftServer(DiscordClient client, MinecraftServerConfiguration config) : base(client, config)
    {
        this.config = config;
    }

    public async Task<PingInformation?> GetPingInformation()
    {
        MinecraftPingHelper helper = new (config.Port);
        return await helper.GetPingInformation();
    }
}
