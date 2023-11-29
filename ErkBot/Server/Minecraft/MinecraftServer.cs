using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkBot.Server.Minecraft;
public class MinecraftServer : BaseServer
{
    private Process process;
    private string serverDirectory;
    private string startScriptPath;

    public MinecraftServer(MinecraftServerConfiguration config, DiscordChannel logChannel) : base(logChannel, config)
    {
        serverDirectory = config.ServerDirectory;
        startScriptPath = config.StartScriptPath;
    }

    public override void Kill()
    {
        throw new NotImplementedException();
    }

    public override Task Start(double timeOut)
    {
        throw new NotImplementedException();
    }

    public override Task Stop(double timeOut)
    {
        throw new NotImplementedException();
    }
}
