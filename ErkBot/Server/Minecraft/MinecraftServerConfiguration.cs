using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErkBot.Server.Minecraft;
public class MinecraftServerConfiguration : ServerConfiguration
{
    public string ServerDirectory { get; set; }
    public string StartScriptPath { get; set; }
}
