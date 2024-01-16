using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using ErkBot.Discord.Commands;
using ErkBot.Discord.Logging;
using ErkBot.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ErkBot.Discord;
public class BotClient
{
    private readonly DiscordClient client;

    private readonly Configuration config;

    public ServerManager ServerManager { get; }

    public BotClient(Configuration config)
    {
        this.config = config;
        var discordConfig = new DiscordConfiguration
        {
            Token = config.DiscordToken,
            Intents = DiscordIntents.All,
            MinimumLogLevel = LogLevel.Information
        };
        client = new DiscordClient(discordConfig);
        ServerManager = new ServerManager(config.Servers);

        var services = new ServiceCollection().AddSingleton(ServerManager).BuildServiceProvider();
        var commandConfig = new CommandsNextConfiguration
        {
            Services = services,
            CaseSensitive = false,
            StringPrefixes = new string[] { config.Prefix.ToString() }
        };
        var commands = client.UseCommandsNext(commandConfig);
        commands.SetHelpFormatter<HelpFormatter>();
        commands.RegisterCommands<MiscCommands>();
        commands.RegisterCommands<ServerCommands>();
    }

    public async Task Start()
    {
        // setup logging
        var logChannel = await client.GetChannelAsync(config.LogChannelId);
        var logger = new DiscordLogger(logChannel);
        logger.Register();
        foreach(var server in ServerManager.Servers)
        {
            var channel = await client.GetChannelAsync(server.LogChannelId);
            var serverLogger = new ServerToDiscordLogger(server, channel);
            serverLogger.Start();
        }

        DiscordActivity activity = new("You", ActivityType.Watching);
        await client.ConnectAsync(activity);
        Logger.Log(LogLevel.Information, "Hello");

        // start the servers
        ServerManager.StartAll();
    }
}
