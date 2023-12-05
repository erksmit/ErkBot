using System.Diagnostics;

using DSharpPlus;
using DSharpPlus.Entities;
using log4net;

namespace ErkBot.Server;
public abstract class BaseServer
{
    public string DisplayName { get; }
    public bool Enabled { get; }
    public DiscordChannel LogChannel { get; private set; }
    public ServerStatus Status { get; private set; }

    protected BaseServer(DiscordClient client, ServerConfiguration config)
    {
        log = LogManager.GetLogger(typeof(BaseServer));
        Status = ServerStatus.Stopped;
        DisplayName = config.Name;
        Enabled = config.Enabled;
        logChannelId = config.OutputChannelId;
        discordClient = client;
        
        var serverDirectory = config.ServerDirectory;
        var startScriptPath = config.StartScriptPath;
        Process = new Process
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
        Process.OutputDataReceived += ServerOutputReceived;
        Process.Exited += ServerExited;

        MessageReceived += SendReceivedMessageToDiscord;
    }

    private readonly DiscordClient discordClient;
    private readonly ulong logChannelId;
    private readonly ILog log;

    protected readonly Process Process;
    protected BufferedDiscordChannel OutputChannel;

    public async Task<bool> Start()
    {
        return await Task.Run(async () =>
        {
            try
            {
                if(LogChannel == null)
                {
                    LogChannel = await discordClient.GetChannelAsync(logChannelId);
                    OutputChannel = new BufferedDiscordChannel(LogChannel);
                    OutputChannel.Start();
                }

                Process.Start();
                Status = ServerStatus.Running;
                log.Info($"Started server: {DisplayName}");
                return true;
            }
            catch(Exception e)
            {
                log.Error($"Failed to start server: {DisplayName}", e);
                return false;
            }
        });
    }

    public async Task<bool> Stop(int timeOut = 10_000) => await Task.Run(() => Process.WaitForExit(timeOut));
    
    public event EventHandler<ServerMessageReceivedEventArgs> MessageReceived;

    protected virtual void ServerOutputReceived(object sender, DataReceivedEventArgs e)
    {
        string message = e.Data;
        if (message != null)
        {
            OnMessageReceived(this, new ServerMessageReceivedEventArgs(message));
        }
    }

    private void ServerExited(object sender, EventArgs e)
    {
        int exitCode = Process.ExitCode;
        if (exitCode == 0)
        {
            Status = ServerStatus.Stopped;
            log.Info($"Server {DisplayName} has exited gracefully");
        }
        else
        {
            Status = ServerStatus.Crashed;
            log.Warn($"Server {DisplayName} has exited with exitcode {exitCode}");
        }
    }

    protected void OnMessageReceived(object sender, ServerMessageReceivedEventArgs args)
    {
        MessageReceived?.Invoke(sender, args);
    }

    private void SendReceivedMessageToDiscord(object sender, ServerMessageReceivedEventArgs args)
    {
        OutputChannel.QueueMessage(args.Message);
    }
}
