using ErkBot.Server.Configuration;
using ErkBot.Server.Types.Minecraft.Model;

namespace ErkBot.Server.Types.Minecraft;

/// <summary>
/// A minecraft java server.
/// </summary>
public class MinecraftServer : ExecutableServer
{
    private readonly MinecraftServerConfiguration config;
    public MinecraftServer(MinecraftServerConfiguration config) : base(config)
    {
        this.config = config;
    }

    /// <summary>
    /// Retrieves information about the minecraft server via the ping protocol.
    /// </summary>
    /// <returns>A <see cref="PingInformation"/> containing information about the server.</returns>
    public async Task<PingInformation?> GetPingInformation()
    {
        MinecraftPingHelper helper = new (config.Port);
        return await helper.GetPingInformation();
    }
}
