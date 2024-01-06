using discord_bot;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using ErkBot.Command;
using ErkBot.Server;
using ErkBot.Server.Configuration;
using ErkBot.Server.Types;
using ErkBot.Server.Types.Minecraft;
using Microsoft.Extensions.Logging;

namespace ErkBot;
public class Client
{
    private readonly DiscordClient client;

    private readonly Configuration config;

    public List<BaseServer> Servers { get; }

    public Client(Configuration config)
    {
        this.config = config;
        var discordConfig = new DiscordConfiguration
        {
            Token = config.DiscordToken,
            Intents = DiscordIntents.All,
            MinimumLogLevel = LogLevel.Information
        };
        client = new DiscordClient(discordConfig);
        var commandConfig = new CommandsNextConfiguration
        {
            CaseSensitive = false,
            StringPrefixes = new string[] { config.Prefix.ToString() },
        };
        var commands = client.UseCommandsNext(commandConfig);
        commands.SetHelpFormatter<HelpFormatter>();
        commands.RegisterCommands<MiscCommands>();

        Servers = new List<BaseServer>();
        foreach (var serverConfig in config.Servers)
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

    public async Task Start()
    {
        DiscordActivity activity = new("You", ActivityType.Watching);
        await client.ConnectAsync(activity);

        var logChannel = await client.GetChannelAsync(config.LogChannelId);
        Logger.Initialize(logChannel);
        await Logger.InfoAsync("Hello");

        var tasks = Servers.Where(s => s.Enabled).Select(s => s.Start()).ToArray();
        Task.WaitAll(tasks);

        //await Logger.InfoAsync("Startup finished");
    }
}
