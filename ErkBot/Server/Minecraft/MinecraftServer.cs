using DSharpPlus;
using System.Diagnostics;

namespace ErkBot.Server.Minecraft;
public class MinecraftServer : BaseServer
{
    private readonly Process process;
    private readonly string serverDirectory;
    private readonly string startScriptPath;

    public MinecraftServer(DiscordClient client, MinecraftServerConfiguration config) : base(client, config)
    {
        serverDirectory = config.ServerDirectory;
        startScriptPath = config.StartScriptPath;
        process = new Process
        {
            StartInfo =
            {
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                CreateNoWindow = true,
                FileName = startScriptPath,
                WorkingDirectory = serverDirectory,
            },
            
        };

    }

    public override event EventHandler<ServerMessageReceivedEventArgs>? MessageReceived;

    public override bool Kill()
    {
        try
        {
            process.Kill();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async override Task<bool> Start()
    {
        return await Task.Run(async () =>
        {
            try
            {
                if(LogChannel == null)
                {
                    LogChannel = await discordClient.GetChannelAsync(logChannelId);
                }

                process.Start();
                return true;
            }
            catch
            {
                return false;
            }
        });
    }

    public async override Task<bool> Stop(int timeOut)
    {
        return await Task.Run(() =>
        {
            return process.WaitForExit(timeOut);
        });
    }
}
