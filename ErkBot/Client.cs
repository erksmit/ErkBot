using DSharpPlus;
using DSharpPlus.Entities;
using ErkBot.Server;
using ErkBot.Server.Minecraft;

namespace ErkBot;
public class Client
{
    private DiscordClient client;

    private DiscordChannel logChannel;

    private Configuration config;

    public List<BaseServer> Servers { get; set; }

    
    public Client(Configuration config)
    {
        var discordConfig = new DiscordConfiguration
        {
            Token = config.DiscordToken,
        };
        client = new DiscordClient(discordConfig);

        Servers = new List<BaseServer>();
        foreach(ServerConfiguration serverConfig in config.Servers) 
        { 
            switch(serverConfig.Type)
            {
                case ServerType.Minecraft:
                    {
                        var minecraftServerConfig = serverConfig as MinecraftServerConfiguration ?? throw new ConfigurationException("Minecraft server configuration is not valid");
                        var server = new MinecraftServer(client, minecraftServerConfig);
                        break;
                    }
                default:
                    {
                        throw new ConfigurationException("Unknown server type configured");
                    }
            }
        }
    }

    public async Task Start()
    {
        logChannel = await client.GetChannelAsync(config.LogChannelId);
        var tasks = Servers.Where(s => s.Enabled).Select(s => s.Start()).ToArray();
        var results = await Task.WhenAll(tasks);
        if (results.Any(b => !b))
        {

        }
    }


}
