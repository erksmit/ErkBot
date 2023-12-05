using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using DSharpPlus.Entities;

namespace discord_bot;

class Program
{
    public static MainForm Form;
    [STAThread]
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Task.Run(() =>
        {
            Form = new MainForm();
            Application.Run(Form);
        });
        while (true)
        {
            try
            {
                MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();//its gotta be async or dsharpplus functions wont run async
            }
            catch (Exception e)
            {
                Form.AddToTextBox($"a major error has occured: {e.Message}\n{e.StackTrace}", Form.systemOutputTextbox);
                Console.Write("[ERROR] ");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
#if DEBUG
                throw e;
#endif
                Config.Discord.SendMessageAsync(Config.OutputChannel, "\n [ERROR] something big just broke, restarting \n" + e.Message + "\n if youre seeing this, please ping me \n");
            }
        }
    }

    static async Task MainAsync(string[] args)
    {
        Config.InitializeConfig();
        Console.WriteLine("config loaded, startup time: " + Config.startupTime);
        Form.AddToTextBox($"config loaded, startup time: {Config.startupTime}", Form.systemOutputTextbox);


        await InitializeServer();
        InitializeTreadsAndEvents();
        SendMessage(new Message(Config.OutputChannel, "startup ok"));
        SendMessage(new Message(Config.OutputChannel, "hello world!"));
        while (true)
        {
            string message = Console.ReadLine();
            if (message.Length != 0)
            {
                if (message == "stop")
                {
                    await Config.OutputChannel.SendMessageAsync("im outta here scoob");
                    foreach (Server server in Config.Servers)
                    {
                        try
                        {
                            server.server.StandardInput.WriteLine("stop");
                        }
                        catch { }
                    }
                    await Config.Discord.DisconnectAsync();
                    Environment.Exit(0);
                }
                else
                {
                    SendMessage(new Message(Config.OutputChannel, message));
                }
            }
        }
    }
    private static void InitializeTreadsAndEvents()
    {
        new Thread(new ThreadStart(MessageQueueThread)).Start();
        Console.WriteLine("[INFO] threads started");
        Form.AddToTextBox("Threads started", Form.systemOutputTextbox);
    }


    //TODO decide to use this or not
    private static async Task InitializeServer()
    {
        foreach (var server in Config.Servers)
        {
            server.Start();
        }
    }

    public static void SendMessage(Message message)//TODO messages >2000 chars can get out of order
    {   //null message can be sent on server shutdown
        if (message.Content != null)
        {
            Message recursionMsg = null;
            //cut up message if ulonger than 2000 characters
            if (message.Content.Length > 2000)
            {
                recursionMsg = new Message(message.Destination, message.Content.Substring(2000));
                message.Content = message.Content.Substring(0, 2000);
            }
            message.Enqueue();
            Console.WriteLine("[SENT] " + message.Content);
            if (message.Content.Length > 2000)
            {
                SendMessage(recursionMsg);
            }
        }
        else
        {
            Console.WriteLine("message without content received, ignoring");
            Form.AddToTextBox("message without content received, ignoring", Form.systemOutputTextbox);
        }
    }
    public static Message SendMessage(DiscordChannel destination, string content)
    {
        var message = new Message(destination, content);
        SendMessage(message);
        return message;
    }

    //guildid, pending messages in guild, channelid, pending messages for channel, messages
    public static Dictionary<ulong, Dictionary<ulong, Queue<Message>>> pendingMessages = new Dictionary<ulong, Dictionary<ulong, Queue<Message>>>();


    public static readonly object pendingMessagesLock = new object();
    public static EventWaitHandle MsgQueueWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

    public static ConcurrentQueue<Tuple<ulong, string>> readyMessages = new ConcurrentQueue<Tuple<ulong, string>>();
    public static EventWaitHandle readyMsgWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

    static void MessageQueueThread()
    {
        while (true)
        {
            MsgQueueWaitHandle.WaitOne();
            lock (pendingMessagesLock)
            {
                try
                {
                    while (pendingMessages.Count > 0)
                    {
                        KeyValuePair<ulong, Queue<Message>> pendingChannel = default;
                        KeyValuePair<ulong, Dictionary<ulong, Queue<Message>>> pendingServer = default;
                        //find the channel with the most pending messages
                        foreach (var server in pendingMessages)
                        {
                            foreach(var channel in server.Value)
                            {
                                if (pendingChannel.Value == null)
                                    pendingChannel = channel;
                                else if (channel.Value.Count > pendingChannel.Value.Count)
                                    pendingChannel = channel;
                            }
                            if (pendingServer.Value == null)
                                pendingServer = server;
                            else if (server.Value.ContainsKey(pendingChannel.Key))
                                pendingServer = server;
                        }

                        if (pendingChannel.Value.Count > 0)
                        {
                            Message message = pendingChannel.Value.Dequeue();
                            //combine messages until full or no more messages are left
                            while (true)
                            {
                                if (pendingChannel.Value.Count > 0)
                                {
                                    if (message.Combine(pendingChannel.Value.Peek()))
                                        pendingChannel.Value.Dequeue();
                                    else
                                        break;
                                }
                                else
                                {   //remove channel if all pending messages have been sent
                                    pendingServer.Value.Remove(pendingChannel.Key);
                                    //remove server if all channels are gone
                                    if (pendingServer.Value.Count == 0)
                                        pendingMessages.Remove(pendingServer.Key);
                                    break;
                                }
                            }

                            message.Send();
                        }
                    }
                }
                catch (Exception r)
                {
                    Config.OutputChannel.SendMessageAsync("queuemanager broke, this bad \n" + r.Message);
                    Form.AddToTextBox($"an error has occured in the queuemanager\n{r.Message}", Form.systemOutputTextbox);
                }
            }
            Thread.Sleep(2000);
        }
    }
}