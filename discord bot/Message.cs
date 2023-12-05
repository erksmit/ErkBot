using System.Collections.Generic;

using DSharpPlus.Entities;

namespace discord_bot;

class Message
{
    private static int _pendingMessages;
    public static int PendingMessages
    {
        get { return _pendingMessages; }
        set {
            _pendingMessages = value;
            Program.Form.SetPendingMessages(_pendingMessages);
        }
    }

    public string Content;
    public DiscordChannel Destination;
    public bool Sent;
    public List<Message> CombinedWith;


    public Message(DiscordChannel destination, string content)
    {
        Sent = false;
        Content = content;
        Destination = destination;
    }

    public void Enqueue()
    {
        lock (Program.pendingMessagesLock)
        {
            var server = Destination.Guild;
            //failsafe
            if(server != null)
            {
                //check server exists
                if (!Program.pendingMessages.ContainsKey(server.Id))
                {
                    Program.pendingMessages.Add(server.Id, new Dictionary<ulong, Queue<Message>>());
                }
                //check channel exists
                if (!Program.pendingMessages[server.Id].ContainsKey(Destination.Id))
                {
                    Program.pendingMessages[server.Id].Add(Destination.Id, new Queue<Message>());
                }
                //channel and server exist now
                Program.pendingMessages[server.Id][Destination.Id].Enqueue(this);//queue the message itself
                //start the message queue
                PendingMessages++;
                Program.MsgQueueWaitHandle.Set();
            }
        }
    }

    public bool Combine(Message otherMessage)
    {
        if (CombinedWith == null)
            CombinedWith = new List<Message>();
        if (otherMessage.Content.Length + Content.Length > 2000 - 2)
            return false;
        Content += "\n" + otherMessage.Content;
        CombinedWith.Add(otherMessage);
        return true;
    }

    public void Send()
    {
        Config.Discord.SendMessageAsync(Destination, Content);
        PendingMessages--;
        if (CombinedWith != null)
        {
            foreach (Message message in CombinedWith)
            {
                message.Sent = true;
            }
            PendingMessages -= CombinedWith.Count;
        }
        Sent = true;
    }
}