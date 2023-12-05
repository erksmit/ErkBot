using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Microsoft.Win32;

namespace discord_bot
{
    public class Commands : BaseCommandModule
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public static async Task Errored(object sender, CommandErrorEventArgs e)
        {
            Program.SendMessage(new Message(e.Context.Channel, "an error occured in executing the command \n```fix \n" + e.Exception.Message + "```"));
        }

        public override async Task BeforeExecutionAsync(CommandContext e)
        {
            Program.Form.AddToTextBox($"user \"{e.User.Username}\" has used command \"{e.Command.Name}\" with arguments \"{e.RawArgumentString}\"", Program.Form.systemOutputTextbox);
        }

        [Command("servers"), Description("lists all servers run by this bot")]
        public async Task Servers(CommandContext e)
        {
            var reply = "info: \n";
            foreach (var server in Config.Servers)
            {
                reply += $"    name: \"{server.serverName}\" running: {!server.server.HasExited}\n";
            }
            Program.SendMessage(new Message(e.Channel, reply));
        }

        [Command("uptime"), Description("get the uptime of the bot")]
        public async Task Uptime(CommandContext e, string usefull = null)
        {
            string message = "uptime: ";
            if(usefull == "pls")
            {
                message += ((float)(DateTime.Now - Config.startupTime).Ticks / TimeSpan.TicksPerHour) + " hours";
                message += "\nbecause you asked nicely";
            }
            else
                message = GetUptime(message);
            Program.SendMessage(new Message(e.Channel, message));
        }

        public static async Task Success(object sender, CommandExecutionEventArgs e)
        {
            //TODO implement
        }

        private static string GetUptime(string message)
        {
            TimeSpan uptime = DateTime.Now - Config.startupTime;
            var r = new Random();
            switch (r.Next(10))
            {
                case 1:
                    {
                        message += uptime.Ticks + " ticks";
                        break;
                    }
                case 2:
                    {
                        message += ((decimal)uptime.Ticks / TimeSpan.TicksPerDay / 365 / 1000) + " millenia";
                        break;
                    }
                case 3:
                    {
                        message += ((decimal)uptime.Ticks / TimeSpan.TicksPerDay) + " days";
                        break;
                    }
                case 4:
                    {
                        message += ((decimal)uptime.Ticks / TimeSpan.TicksPerHour) + " hours";
                        break;
                    }
                case 5:
                    {
                        message += ((decimal)uptime.Ticks / TimeSpan.TicksPerMinute) + " minutes";
                        break;
                    }
                case 6:
                    {
                        message += ((decimal)uptime.Ticks / TimeSpan.TicksPerMillisecond + " milliseconds");
                        break;
                    }
                case 7:
                    {
                        message += ((decimal)uptime.Ticks / TimeSpan.TicksPerSecond + " seconds");
                        break;
                    }
                case 8:
                    {
                        message += ((decimal)uptime.Ticks / TimeSpan.TicksPerDay / 365 / 100) + " centuries";
                        break;
                    }
                case 9:
                    {
                        message += ((decimal)uptime.Ticks / 1000) + " microseconds";
                        break;
                    }
                default:
                    {
                        message = "wouldnt you like to know wheather boy";
                        break;
                    }
            }

            return message;
        }

        [Command("info"), Description("displays information about the server hardware")]
        public async Task Info(CommandContext e)
        {
            Program.SendMessage(new Message(e.Channel, $@"
        hardware information:
            current os: {Environment.OSVersion.Platform},
            core count: {Environment.ProcessorCount},
            {GetUptime("uptime: ")},
            currently shutting down: {Environment.HasShutdownStarted}   duh,
            processor name: {Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0", RegistryKeyPermissionCheck.ReadSubTree).GetValue("ProcessorNameString")},
            currently consuming {Math.Round(Process.GetCurrentProcess().PrivateMemorySize64 / 1024f / 1024f)} MB of memory and counting,
            "));
        }


        [Command("start"), Description("if the server is down this will start it")]
        public async Task Start(CommandContext e, string serverName = null)
        {
            if (string.IsNullOrWhiteSpace(serverName))
            {
                foreach (var server in Config.Servers)
                {
                    if (server.Restart())
                        Program.SendMessage(new Message(e.Channel, $"server \"{server.serverName}\" has been started"));
                    else
                        Program.SendMessage(new Message(e.Channel, $"server \"{server.serverName}\" is already running"));
                }
                Program.SendMessage(new Message(e.Channel, "\n done"));
            }
            else
            {
                var server = Config.Servers.Find(x => x.serverName == serverName);
                if (server != null)
                {
                    if (server.Restart())
                        Program.SendMessage(new Message(e.Channel, "server has been started"));
                    else
                        Program.SendMessage(new Message(e.Channel, "server is already running"));
                }
            }
        }

        [Command("flush"), Description("deletes all pending messages, use if the bot is stuck sending a large amount of messages"), RequirePermissions(Permissions.ManageGuild)]
        public async Task Flush(CommandContext e)
        {
            lock(Program.pendingMessagesLock)
            {
                Program.pendingMessages.Clear();
            }
            Program.SendMessage(new Message(e.Channel, e.Message.Author.Username + " flushed all pending messages"));
        }

        [Command("kill"), Description("shuts down a server, specify a server name to shut down a specific server"), RequirePermissions(Permissions.ManageGuild)]
        public async Task Kill(CommandContext e, string serverName = null)
        {
            var tasks = new List<Task>();
            if (serverName == null)
            {
                foreach (var s in Config.Servers)
                {
                    if (s != null)
                    {
                        tasks.Add(KillServer(e, s));
                    }
                }
                await Task.WhenAll(tasks.ToArray());
            }
            else
            {
                var s = Config.Servers.Find(x => x.serverName == serverName);
                if (s != null)
                {
                    await KillServer(e, s);
                }
                else
                    Program.SendMessage(e.Channel, "server not found");
            }
            Program.SendMessage(e.Channel, "done");
        }

        private Task KillServer(CommandContext e, Server s)
        {
            return Task.Run(() =>
            {
                try
                {
                    Program.SendMessage(new Message(e.Channel, $"attempting to kill server \"{s.serverName}\""));
                    if (s is MineCraftServer mineCraftServer)
                    {
                        s.server.StandardInput.WriteLine("stop");
                        Thread.Sleep(10000);
                        if (s.server.HasExited != true)
                            s.server.Kill();
                    }
                    s.server.Dispose();
                    s.server = null;
                    Program.SendMessage(new Message(e.Channel, $"killed server \"{s.serverName}\""));
                }
                catch
                {
                    Program.SendMessage(new Message(e.Channel, $"failed to kill server \"{s.serverName}\""));
                }
            });
        }

        [Command("debug"), Description("lists information usefull for debugging"), RequirePermissions(Permissions.ManageGuild)]
        public async Task Debug(CommandContext e)
        {
            Program.SendMessage(new Message(e.Channel, $"msgQueueWaitHandle set: {Program.MsgQueueWaitHandle.WaitOne(0)}"));
            Program.MsgQueueWaitHandle.Set();//because waitone will reset it
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
