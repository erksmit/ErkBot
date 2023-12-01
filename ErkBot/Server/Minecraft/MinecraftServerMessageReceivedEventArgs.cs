using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkBot.Server.Minecraft;
public class MinecraftServerMessageReceivedEventArgs : ServerMessageReceivedEventArgs
{
    public MinecraftServerMessageReceivedEventArgs(string message) : base(message)
    {
    }
}
