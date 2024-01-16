using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace ErkBot.Discord.Logging;
internal class DiscordLogger
{
    private readonly DiscordChannel channel;

    private const LogLevel minimumLevel = LogLevel.Information;

    public DiscordLogger(DiscordChannel channel)
    {
        this.channel = channel;
    }

    public void Register()
    {
        Logger.LogEvent += OnLog;
    }

    private async void OnLog(object? sender, LogEventArgs e)
    {
        var level = e.Level;
        if (level < minimumLevel)
            return;

        if (level <= LogLevel.Information)
        {
            await channel.SendMessageAsync(e.Message);
        }
        else
        {
            DiscordEmbedBuilder embedBuilder = new()
            {
                Description = e.Message,
                Color = level switch
                {
                    LogLevel.Warning => DiscordColor.Yellow,
                    LogLevel.Error => DiscordColor.Red,
                    LogLevel.Critical => DiscordColor.DarkRed,
                    _ => DiscordColor.White,
                }
            };
            await channel.SendMessageAsync(embedBuilder.Build());
        }
    }
}
