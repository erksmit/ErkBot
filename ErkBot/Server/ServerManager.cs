using DSharpPlus;
using ErkBot.Server.Configuration;
using ErkBot.Server.Types;
using ErkBot.Server.Types.Minecraft;

namespace ErkBot.Server;
public class ServerManager
{
    public List<BaseServer> Servers { get; private set; }

    internal ServerManager(BaseServerConfiguration[] config)
    {
        Servers = new List<BaseServer>();
        foreach (var serverConfig in config)
        {
            // skip creating servers that are not enabled
            if (!serverConfig.Enabled)
                continue;

            switch (serverConfig.Type)
            {
                case ServerType.Minecraft:
                    {
                        var server = new MinecraftServer((MinecraftServerConfiguration)serverConfig);
                        Servers.Add(server);
                        break;
                    }
                case ServerType.Fake:
                    {
                        var server = new FakeServer(serverConfig);
                        Servers.Add(server);
                        break;
                    }
                case ServerType.Executable:
                    {
                        var server = new ExecutableServer((ExecutableServerConfiguration)serverConfig);
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

    public void StartAll()
    {
        foreach (var server in Servers)
        {
            server.Start();
        }
    }

    public async Task StopAll(int timeout = 10_000)
    {
        await Task.WhenAll(Servers.Select(s => s.Stop(timeout)));
    }
}
