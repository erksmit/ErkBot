using DSharpPlus;
using DSharpPlus.Entities;
using ErkBot.Server;
using ErkBot.Server.Minecraft;

using log4net;

namespace ErkBot;
public class Client
{
    private DiscordChannel logChannel;

    private readonly DiscordClient client;

    private readonly Configuration config;

    private readonly ILog log;

    public List<BaseServer> Servers { get; }


    
    public Client(Configuration config)
    {
        this.config = config;
        log = LogManager.GetLogger(typeof(Client));
        var discordConfig = new DiscordConfiguration
        {
            Token = config.DiscordToken,
        };
        client = new DiscordClient(discordConfig);

        Servers = new List<BaseServer>();
        foreach(var serverConfig in config.Servers) 
        { 
            switch(serverConfig.Type)
            {
                case ServerType.Minecraft:
                    {
                        var server = new MinecraftServer(client, serverConfig);
                        Servers.Add(server);
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
