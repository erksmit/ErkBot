using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using ErkBot.Discord.Commands;
using ErkBot.Server;
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
        var commandConfig = new CommandsNextConfiguration
        {
            CaseSensitive = false,
            StringPrefixes = new string[] { config.Prefix.ToString() },
        };
        var commands = client.UseCommandsNext(commandConfig);
        commands.SetHelpFormatter<HelpFormatter>();
        commands.RegisterCommands<MiscCommands>();

        ServerManager = new ServerManager(client, config.Servers);
    }

    public async Task Start()
    {
        var logChannel = await client.GetChannelAsync(config.LogChannelId);
        var logger = new DiscordLogger(logChannel);
        logger.Register();
        DiscordActivity activity = new("You", ActivityType.Watching);
        await client.ConnectAsync(activity);

        Logger.Log(LogLevel.Information, "Hello");
        await ServerManager.StartAll();

        //await Logger.InfoAsync("Startup finished");
    }
}
