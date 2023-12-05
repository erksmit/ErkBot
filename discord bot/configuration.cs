using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using Newtonsoft.Json;

namespace discord_bot;

public enum ServerType
{
    Generic,
    Minecraft
}
static class Config
{
    public static DateTime startupTime;//utc +1
    public static DiscordClient Discord;
    public static DiscordChannel OutputChannel;
    public static List<Server> Servers;
    public static EventWaitHandle GuildDownloadComplete = new EventWaitHandle(false, EventResetMode.ManualReset);
    public static RawConfig rawConfig;

    private static readonly string configPath = Environment.CurrentDirectory + @"\Rescources\config.json";

    public static void InitializeConfig()
    {

        startupTime = DateTime.Now;
        Console.WriteLine("startup time: " + startupTime);

        //load config text
        try
        {
            string configJson = File.ReadAllText(configPath);
            rawConfig = JsonConvert.DeserializeObject<RawConfig>(configJson);
        }
        catch (Exception)
        {
            Console.WriteLine("error loading config, creating default config");
            rawConfig = new RawConfig();
            string configJson = JsonConvert.SerializeObject(rawConfig);
            File.WriteAllText(configPath, configJson);
        }

        ConfigureDiscord(rawConfig).Wait();
        ConfigureServers(rawConfig);
        Program.Form.LoadSelfbot();
        Program.Form.LoadServerTextbox();

        Program.SendMessage(new Message(OutputChannel, "configuration loaded"));

    }
    private static async Task ConfigureDiscord(RawConfig rawConfig)
    {
        Console.WriteLine("configuring discord settings...");
        Discord = new DiscordClient(new DiscordConfiguration
        {
            Token = rawConfig.discordToken,
            Intents = DiscordIntents.All,
            MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Information
        });

        CommandsNextExtension coms = Discord.UseCommandsNext(new CommandsNextConfiguration
        {
            CaseSensitive = false,
            StringPrefixes = new[] { rawConfig.commandPrefix },
            EnableDms = false
        });
        coms.CommandErrored += Commands.Errored;
        coms.CommandExecuted += Commands.Success;
        coms.RegisterCommands<Commands>();
        coms.SetHelpFormatter<CommandHelpFormatter>();

        await Discord.ConnectAsync();
        await Discord.InitializeAsync();
        Discord.GuildDownloadCompleted += (sender, args) =>
        {
            GuildDownloadComplete.Set();
            return Task.CompletedTask;
        };

        OutputChannel = await Discord.GetChannelAsync(rawConfig.systemOutputChannelID);
        if (rawConfig.discordSettings["pongOnPing"])
            Discord.MessageCreated += (sender, args) =>
            {
                if (Regex.Match(args.Message.Content.ToLower(), @"\b(ping)\b").Success)
                {
                    Program.SendMessage(new Message(args.Channel, "pong! " + Discord.Ping + "ms"));
                }
                return Task.CompletedTask;
            };
        if (rawConfig.discordSettings["creeper?"])
            Discord.MessageCreated += (sender, args) =>
            {
                if (args.Message.Content.ToLower().Contains("creeper"))
                    Program.SendMessage(new Message(args.Channel, "aww man"));
                return Task.CompletedTask;
            };
        if (rawConfig.discordSettings["eyesOnEdit"])
            Discord.MessageUpdated += (sender, args) =>
            {
                Program.SendMessage(new Message(args.Channel, " you cannot hide your edits from me :eyes: " + args.Author.Username));
                return Task.CompletedTask;
            };
        if (rawConfig.discordSettings["bruhOnDelete"])
            Discord.MessageDeleted += (sender, args) =>
            {
                if (args.Message.Author.Equals(Discord.CurrentUser) && args.Message.Content != "bruh moment")
                    Program.SendMessage(new Message(args.Channel, "bruh moment"));
                return Task.CompletedTask;
            };
        Discord.UpdateStatusAsync(new DiscordActivity($"with admin powers ({rawConfig.commandPrefix}help for info)"));
    }

    private static Task Discord_GuildDownloadCompleted(DiscordClient sender, DSharpPlus.EventArgs.GuildDownloadCompletedEventArgs e)
    {
        throw new NotImplementedException();
    }

    //load server configs
    private static void ConfigureServers(RawConfig rawConfig)
    {
        Servers = new List<Server>();
        foreach (var (serverType, outputChannelID, startScriptPath, name) in rawConfig.servers)
        {
            Server server = null;
            switch(serverType)
            {
                case ServerType.Generic:
                {
                    Console.WriteLine("creating generic server");
                    server = new GenericServer(outputChannelID, startScriptPath, name);
                    break;
                }
                case ServerType.Minecraft:
                {
                    Console.WriteLine("creating minecraft server");
                    server = new MineCraftServer(outputChannelID, startScriptPath, name);
                    break;
                }
            }
            Console.WriteLine("outputChannelID: " + outputChannelID +
                              "\nstartScriptPath: " + startScriptPath +
                              "\nname: " + name);
            Servers.Add(server);
            Program.Form.AddToTextBox($"loaded server,\noutput channel: {OutputChannel.Name}\nstarting script path:{startScriptPath}\nname: {name}", Program.Form.systemOutputTextbox);
        }
    }

    public static bool ReloadConfig()
    {
        //TODO
        throw new NotImplementedException();
    }

    public class RawConfig
    {
        public string commandPrefix;
        public string discordToken;
        public ulong systemOutputChannelID;
        public Dictionary<string, bool> discordSettings;
        /*
        allowReload
        eyesOnEdit
        pongOnPing
        creeper?
        bruhOnDelete*/
        public (ServerType type, ulong outputChannelID, string startScriptPath, string name)[] servers;

        public RawConfig()
        {
            //TODO make default config here
            commandPrefix = ";";
            discordToken = string.Empty;
            systemOutputChannelID = 0;
            discordSettings = new Dictionary<string, bool>
            {
                { "allowReload", false },
                { "eyesOnEdit", false },
                { "pongOnPing", true },
                { "creeper?", false },
                { "bruhOnDelete", false }
            };
            servers = new (ServerType type, ulong outputChannelID, string startScriptPath, string name)[]
            {
                (ServerType.Generic, 0, "path to start script here", "example server")
            };
        }
    }
}