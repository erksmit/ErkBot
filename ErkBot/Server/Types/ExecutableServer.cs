using ErkBot.Server.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ErkBot.Server.Types;
public class ExecutableServer : BaseServer
{
    public ExecutableServer(ExecutableServerConfiguration config) : base(config)
    {
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
                WorkingDirectory = serverDirectory ?? Path.GetDirectoryName(startScriptPath),
            },
        };
        Process.OutputDataReceived += ServerOutputReceived;
        Process.Exited += ServerExited;
    }

    protected readonly Process Process;

    /// <inheritdoc/>
    public override bool Start()
    {
        try
        {
            Process.Start();
            Status = ServerStatus.Running;
            Logger.Log(LogLevel.Information, $"Server {DisplayName} has started");
            return true;
        }
        catch (Exception e)
        {
            Logger.Log(LogLevel.Warning, $"Server {DisplayName} has failed to start because {e.GetType().Name}: {e.Message}");
            Status = ServerStatus.Crashed;
            return false;
        }
    }

    /// <inheritdoc/>
    public override async Task Stop(int timeOut = 10_000) => await Task.Run(() => Process.WaitForExit(timeOut));

    protected virtual void ServerOutputReceived(object? sender, DataReceivedEventArgs e)
    {
        string? message = e.Data;
        if (message != null)
        {
            OnMessageReceived(message);
        }
    }

    private void ServerExited(object? sender, EventArgs e)
    {
        int exitCode = Process.ExitCode;
        if (exitCode == 0)
        {
            Status = ServerStatus.Stopped;
        }
        else
        {
            Status = ServerStatus.Crashed;
        }
    }
}
