using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Diagnostics;

namespace ErkBot.Command;
public class MiscCommands : BaseCommandModule
{

    [Command("uptime"), Description("gets the uptime of the bot")]
    public async Task Uptime(CommandContext e, string? askingNicely = null)
    {
        string message = "uptime: ";
        if (askingNicely == "pls")
        {
            var timespan = DateTime.Now - Process.GetCurrentProcess().StartTime;
            message += timespan.ToString("g");
            message += "\nbecause you asked nicely";
        }
        else
            message = GetUptime(message);
        await e.RespondAsync(message);
    }


    private static string GetUptime(string message)
    {
        TimeSpan uptime = DateTime.Now - Process.GetCurrentProcess().StartTime;
        var r = new Random();

        switch (r.Next(10))
        {
            case 1:
                message += uptime.Ticks + " ticks";
                break;
            case 2:
                message += ((decimal)uptime.Ticks / TimeSpan.TicksPerDay / 365 / 1000) + " millenia";
                break;
            case 3:
                message += ((decimal)uptime.Ticks / TimeSpan.TicksPerDay) + " days";
                break;
            case 4:
                message += ((decimal)uptime.Ticks / TimeSpan.TicksPerHour) + " hours";
                break;
            case 5:
                message += ((decimal)uptime.Ticks / TimeSpan.TicksPerMinute) + " minutes";
                break;
            case 6:
                message += ((decimal)uptime.Ticks / TimeSpan.TicksPerMillisecond + " milliseconds");
                break;
            case 7:
                message += ((decimal)uptime.Ticks / TimeSpan.TicksPerSecond + " seconds");
                break;
            case 8:
                message += ((decimal)uptime.Ticks / TimeSpan.TicksPerDay / 365 / 100) + " centuries";
                break;
            case 9:
                message += ((decimal)uptime.Ticks / 1000) + " microseconds";
                break;
            default:
                message = "wouldnt you like to know, wheather boy";
                break;
        }

        return message;
    }
}
