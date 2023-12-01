using DSharpPlus;
using DSharpPlus.Entities;
using ErkBot.Server;
using ErkBot.Server.Minecraft;

namespace ErkBot;
public class Client
{
    private DiscordClient client;

    private DiscordChannel logChannel;
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
                        var server = new MinecraftServer(client, serverConfig as MinecraftServerConfiguration);
                        break;
                    }
                default:
                    {
                        throw new ConfigurationException("Unknown server type configured");
                    }
            }
        }
    }

    public void Start()
    {

    }


}
