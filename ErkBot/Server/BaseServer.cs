using DSharpPlus.Entities;
using ErkBot.Server.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkBot.Server;
public abstract class BaseServer
{
    public string DisplayName { get; }
    public DiscordChannel LogChannel { get; }

    public BaseServer(DiscordChannel logChannel, string displayName)
    {
        LogChannel = logChannel;
        DisplayName = displayName;
    }

    public event EventHandler<MessageRecievedEventArgs> MessageRecieved;



    public abstract Task Start(double timeOut);

    public abstract Task Stop(double timeOut);

    public abstract void Kill();
}
