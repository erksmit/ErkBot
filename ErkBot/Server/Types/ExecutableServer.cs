﻿using DSharpPlus;
using ErkBot.Discord;
using ErkBot.Server.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ErkBot.Server.Types;
public class ExecutableServer : BaseServer
{
    public ExecutableServer(DiscordClient client, ExecutableServerConfiguration config) : base(client, config)
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

    public override async Task Start()
    {
        await base.Start();
        await Task.Run(async () =>
        {
            try
            {
                Process.Start();
                Status = ServerStatus.Running;

                Logger.Log(LogLevel.Information, $"Server {DisplayName} has started");
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Warning, $"Server {DisplayName} has failed to start because {e.GetType().Name}: {e.Message}");
            }
        });
    }

    public override async Task Stop(int timeOut = 10_000) => await Task.Run(() => Process.WaitForExit(timeOut));

    protected virtual void ServerOutputReceived(object? sender, DataReceivedEventArgs e)
    {
        string? message = e.Data;
        if (message != null)
        {
            OnMessageReceived(this, new ServerMessageReceivedEventArgs(message));
        }
    }

    private void ServerExited(object? sender, EventArgs e)
    {
        int exitCode = Process.ExitCode;
        if (exitCode == 0)
        {
            Status = ServerStatus.Stopped;
            Logger.Log(LogLevel.Information, $"Server {DisplayName} has exited gracefully.");
        }
        else
        {
            Status = ServerStatus.Crashed;
            Logger.Log(LogLevel.Warning, $"Server {DisplayName} broke.");
        }
    }
}
