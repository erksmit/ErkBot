using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using ErkBot.Server;
using ErkBot.Server.Types.Minecraft;
using System.Text;

namespace ErkBot.Discord.Commands;

internal class ServerCommands : BaseCommandModule
{
    private readonly ServerManager serverManager;
    public ServerCommands(ServerManager manager)
    {
        serverManager = manager;
    }

    private BaseServer? GetServer(string name)
    {
        return serverManager.Servers.FirstOrDefault(s => string.Equals(s.DisplayName, name, StringComparison.OrdinalIgnoreCase));
    }

    [Command("startServer")]
    [Description("Starts the server with the specified name.")]
    [RequireUserPermissions(Permissions.Administrator)]
    public async Task StartServer(CommandContext e, string serverName)
    {
        var server = GetServer(serverName);
        if (server == null)
        {
            await e.RespondAsync("There is no server with this name.");
            return;
        }

        if (server.Status == ServerStatus.Running)
        {
            await e.RespondAsync("This server is already running!");
        }

        bool success = server.Start();
        if (success)
        {
            await e.RespondAsync("Consider it done.");
        }
        else
        {
            await e.RespondAsync("Uh oh, something didn't go right.");
        }
    }

    [Command("stopServer")]
    [Description("Stops the server with the specified name.")]
    [RequireUserPermissions(Permissions.Administrator)]
    public async Task StopServer(CommandContext e, string serverName)
    {
        var server = GetServer(serverName);
        if (server == null)
        {
            await e.RespondAsync("There is no server with this name.");
            return;
        }

        if (server.Status == ServerStatus.Stopped)
        {
            await e.RespondAsync("This server has already stopped!");
        }

        await server.Stop();
        if (server.Status == ServerStatus.Running)
        {
            await e.RespondAsync("Server didn't stop :(");
        }
        else
        {
            await e.RespondAsync("Consider it done.");
        }
    }

    [Command("serverStatus")]
    [Description("Gets the status of the specified server.")]
    public async Task Status(CommandContext e, string serverName)
    {
        var server = GetServer(serverName);
        if (server == null)
        {
            await e.RespondAsync("There is no server with this name.");
            return;
        }
        var embed = new DiscordEmbedBuilder
        {
            Title = "Server: " + serverName,
        };

        if (server is MinecraftServer minecraftServer && server.Status == ServerStatus.Running)
        {
            var maybeInfo = await minecraftServer.GetPingInformation();
            if (maybeInfo == null)
            {
                embed.Description = "Failed to get detailed info";
            }
            else
            {
                var info = maybeInfo.Value;
                StringBuilder desc = new();
                desc.AppendLine("Version: " + info.Version.Name);
                desc.AppendLine("Players online:");
                if (info.Players.Sample != null && info.Players.Sample.Length != 0)
                {
                    foreach (var player in info.Players.Sample)
                    {
                        desc.AppendLine("\t" + player.Name);
                    }
                }
                else
                    desc.AppendLine("\tNone");
                if (string.IsNullOrEmpty(info.Description.Text))
                    desc.AppendLine(info.Description.Extra?.First().text);
                else
                    desc.AppendLine(info.Description.Text);
                embed.Description = desc.ToString();
            }
        }
        else
        {
            embed.Description = "Status: " + server.Status.AsString() ?? "Unknown";
        }

        await e.RespondAsync(embed.Build());
    }

    [Command("servers")]
    [Description("Gets the names of all servers.")]
    public async Task Servers(CommandContext e)
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = "Servers",
            Description = string.Join('\n', serverManager.Servers.Select(s => s.DisplayName))
        };
        if (embed.Description == string.Empty)
        {
            embed.Description = "There are no servers enabled.";
        }

        await e.RespondAsync(embed.Build());
    }

}
