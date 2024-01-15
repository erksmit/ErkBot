using DSharpPlus;
using ErkBot.Server.Configuration;
using ErkBot.Server.Types;
using ErkBot.Server.Types.Minecraft;

namespace ErkBot.Server;
public class ServerManager
{
    public List<BaseServer> Servers { get; private set; }

    internal ServerManager(DiscordClient client, BaseServerConfiguration[] config)
    {
        Servers = new List<BaseServer>();
        foreach (var serverConfig in config)
        {
            switch (serverConfig.Type)
            {
                case ServerType.Minecraft:
                    {
                        var server = new MinecraftServer(client, (MinecraftServerConfiguration)serverConfig);
                        Servers.Add(server);
                        break;
                    }
                case ServerType.Fake:
                    {
                        var server = new FakeServer(client, serverConfig);
                        Servers.Add(server);
                        break;
                    }
                case ServerType.Executable:
                    {
                        var server = new ExecutableServer(client, (ExecutableServerConfiguration)serverConfig);
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

    public async Task StartAll()
    {
        await Task.WhenAll(Servers.Where(s => s.Enabled).Select(s => s.Start()));
    }

    public async Task StopAll(int timeout = 10_000)
    {
        await Task.WhenAll(Servers.Select(s => s.Stop(timeout)));
    }
}
