using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using DSharpPlus.Entities;

namespace discord_bot;

public abstract class Server
{
    public Process server;
    public DiscordChannel outputChannel;
    public string serverName;

    protected string serverDirectory;
    protected string serverStartLocation;

    protected Server(ulong outputChannelID, string startScriptPath, string name)
    {
        serverStartLocation = startScriptPath;
        serverName = name;
        outputChannel = Config.Discord.GetChannelAsync(outputChannelID).Result;
        serverDirectory = Path.GetDirectoryName(serverStartLocation);
    }

    public bool Start()
    {
        try
        {
            ProcessStartInfo procStartInfo = new ProcessStartInfo()
            {
                FileName = serverStartLocation,
                WorkingDirectory = serverDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Normal,
                UseShellExecute = false,
                CreateNoWindow = false,
                RedirectStandardInput = true
            };
            server = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = procStartInfo
            };
            server.OutputDataReceived += ServerReceivedData;
            server.ErrorDataReceived += ServerReceivedData;
            server.Exited += ShutDown;
            server.Start();
            server.BeginErrorReadLine();
            server.BeginOutputReadLine();
            Program.SendMessage(new Message(Config.OutputChannel, $"started server \"{serverName}\""));
            return true;
        }
        catch(Exception e)
        {
            Program.SendMessage(new Message(Config.OutputChannel, $"failed to start server \"{serverName}\""));
            return false;
        }

    }

    protected async void ShutDown(object sender, EventArgs e)
    {
        Program.Form.AddToTextBox($"server {serverName} has stopped", Program.Form.systemOutputTextbox);
        Console.WriteLine("server machine broke");
        Program.SendMessage(new Message(outputChannel, "server just died RIP \n exitcode: " + server.ExitCode));
        Program.SendMessage(new Message(Config.OutputChannel, $"server \"{serverName}\" has stopped"));
    }
    protected async void ServerReceivedData(object sender, DataReceivedEventArgs e)
    {
        try
        {
            Program.SendMessage(new Message(outputChannel, e.Data));
        }
        catch (Exception r)
        {
            Console.WriteLine(r.Message);
            Console.WriteLine(r.StackTrace);
            Program.Form.AddToTextBox($"an error has occured while sending a message\ncontent: {e.Data}", Program.Form.serverOutputTextbox);
            Program.SendMessage(new Message(outputChannel, "something just went wrong \n" + r.Message));
        }
    }

    public bool Restart()
    {
        if (server == null)
            return Start();
        else if (server.HasExited)
        {
            return Start();
        }
        else
            return false;
    }
}

public class GenericServer : Server
{
    public GenericServer(ulong outputChannelID, string startScriptPath, string name) : base(outputChannelID, startScriptPath, name)
    {
        Config.Discord.MessageCreated += (sender, e) =>
        {
            if (e.Channel == outputChannel)
                if (e.Message.Content.StartsWith("."))//TODO probalby get a customizable prefix for this
                    Program.SendMessage(new Message(outputChannel, "passing commands is not supported in generic server types"));
            return Task.CompletedTask;
        };
    }
}

public class MineCraftServer : Server
{
    public MineCraftServer(ulong outputChannelID, string startScriptPath, string name) : base(outputChannelID, startScriptPath, name)
    {
        Config.Discord.MessageCreated += (sender, args) =>
        {
            if (args.Channel == outputChannel)
                if (args.Message.Content.StartsWith("."))//TODO probalby get a customizable prefix for this
                    if (server != null)
                    {
                        if (server.HasExited == false)
                        {
                            var command = args.Message.Content.Substring(1);
                            Program.Form.AddToTextBox($"user \"{args.Author.Username}\" has used command \"{command}\" in server \"{serverName}\"", Program.Form.systemOutputTextbox);
                            if (command.StartsWith("say "))
                                command = command.Insert(4, $" [{args.Author.Username}] ");
                            Program.SendMessage(new Message(args.Channel, "i gotchu fam"));
                            server.StandardInput.WriteLine("/" + command);
                        }
                        else
                            Program.SendMessage(new Message(args.Channel, "this server is down"));
                    }
                    else
                        Program.SendMessage(new Message(args.Channel, "this server is down"));
            return Task.CompletedTask;
        };
        //coolio logic here
    }
}