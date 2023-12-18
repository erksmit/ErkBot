using DSharpPlus.Entities;

namespace ErkBot;
internal static class Logger
{
    private static DiscordChannel? channel;

    public static void Initialize(DiscordChannel channel)
    {
        Logger.channel = channel;
    }

    public static void Info(string eventName, string? message = null) => InfoAsync(eventName, message).ConfigureAwait(false);

    public static async Task InfoAsync(string eventName, string? message = null)
    {
        if (channel == null)
            throw new Exception("No logging channel is set");
        var builder = GetEmbedBuilder(eventName, message);
        builder.Color = DiscordColor.White;
        await channel.SendMessageAsync(builder.Build());
        Console.WriteLine($"INFO: {eventName} {message}");
    }

    public static void Warn(string eventName, string? message = null) => InfoAsync(eventName, message).ConfigureAwait(false);

    public static async Task WarnAsync(string eventName, string? message = null)
    {
        if (channel == null)
            throw new Exception("No logging channel is set");
        var builder = GetEmbedBuilder(eventName, message);
        builder.Color = DiscordColor.Orange;
        await channel.SendMessageAsync(builder.Build());
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"WARN: {eventName} {message}");
        Console.ForegroundColor = ConsoleColor.White;
    }

    private static DiscordEmbedBuilder GetEmbedBuilder(string eventName, string? message = null)
    {
        DiscordEmbedBuilder embedBuilder = new()
        {
            Title = eventName,
            Description = message
        };
        return embedBuilder;
    }
}
