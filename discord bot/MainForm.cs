using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace discord_bot
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            discordServersListbox.SelectionMode = SelectionMode.One;
            channelsListbox.SelectionMode = SelectionMode.One;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Task.Run(async () =>
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
            }).Wait();
        }

        public void AddToTextBox(string message, RichTextBox textBox)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                if (textBox == this.systemOutputTextbox)
                {
                    var time = DateTime.Now;
                    message = $"[{time.Hour}:{time.Minute}:{time.Second}] " + message;
                }
                textBox.AppendText(message + "\n");
                textBox.ScrollToCaret();
            }));
        }

        public void SetPendingMessages(int count)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                pendingMessagesLabel.Text = "pending messages: " + count;
            }));
        }

        public void LoadServerTextbox()
        {
            Task.Run(() =>
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    Config.GuildDownloadComplete.WaitOne();
                    foreach (Server server in Config.Servers)
                    {
                        serverListbox.Items.Add(server.serverName);
                    }
                    Config.Discord.MessageCreated += UpdateServerTextbox;
                }));
            });
        }

        private Task UpdateServerTextbox(DiscordClient sender, MessageCreateEventArgs e)
        {
            return Task.Run(() =>
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    if (serverListbox.SelectedIndex != -1)
                    {
                        if (e.Channel.Id == Config.Servers[serverListbox.SelectedIndex].outputChannel.Id)
                        {
                            string finalMessage = e.Message.Content;
                            if (e.Author.Username != Config.Discord.CurrentUser.Username)
                                finalMessage = e.Author.Username + ": " + finalMessage;
                            serverOutputTextbox.AppendText(finalMessage + "\n");
                            serverOutputTextbox.ScrollToCaret();
                        }
                    }
                }));
            });
        }

        private void serverListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (serverListbox.SelectedIndex != -1)
            {
                serverOutputTextbox.Lines = new string[0];
                var messages = Config.Servers[serverListbox.SelectedIndex].outputChannel.GetMessagesAsync(100).Result;
                for (int i = messages.Count - 1; i >= 0; i--)
                {
                    var message = messages[i];
                    string finalMessage = message.Content;
                    if (message.Author.Username != Config.Discord.CurrentUser.Username)
                        finalMessage = message.Author.Username + ": " + finalMessage;
                    serverOutputTextbox.AppendText(finalMessage + "\n");
                }
                serverOutputTextbox.ScrollToCaret();
            }
        }


        public async Task LoadSelfbot()
        {
            Task.Run(() =>
            {
                Config.GuildDownloadComplete.WaitOne();
                LoadDiscordServerTextbox();
                Config.Discord.MessageCreated += UpdateChannelContent;
            });
        }

        private KeyValuePair<ulong, DiscordGuild>[] guilds;
        private void LoadDiscordServerTextbox()
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                guilds = new KeyValuePair<ulong, DiscordGuild>[Config.Discord.Guilds.Count];
                int count = 0;
                discordServersListbox.BeginUpdate();
                foreach (var server in Config.Discord.Guilds)
                {
                    guilds[count] = server;
                    discordServersListbox.Items.Add(server.Value.Name);
                    count++;
                }
                discordServersListbox.EndUpdate();
            }));
        }

        private void serversListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (discordServersListbox.SelectedIndex != -1)
            {
                LoadChannelTextBox(guilds[discordServersListbox.SelectedIndex].Value);
            }
        }


        private KeyValuePair<ulong, DiscordChannel>[] channels;
        public void LoadChannelTextBox(DiscordGuild server)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                var localChannels = server.Channels.Where(c => !c.Value.IsCategory && c.Value.Bitrate == 0).ToArray();
                foreach (var channel in server.Channels)
                {
                    if (!channel.Value.IsCategory && channel.Value.Bitrate == 0)
                    {
                        localChannels[channel.Value.Position] = channel;
                    }
                }
                channelsListbox.BeginUpdate();
                channelsListbox.Items.Clear();
                foreach(var channel in localChannels)
                {
                    channelsListbox.Items.Add(channel.Value.Name);
                }
                channelsListbox.EndUpdate();
                channels = localChannels.ToArray();
            }));
        }

        private void channelsListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (discordServersListbox.SelectedIndex != -1)
            {
                LoadChannelContent(channels[channelsListbox.SelectedIndex].Value);
            }
        }

        public void LoadChannelContent(DiscordChannel channel)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                channelContentTextbox.Lines = new string[0];
                var messages = channel.GetMessagesAsync(100).Result;
                for (int i = messages.Count - 1; i >= 0; i--)
                {
                    var message = messages[i];
                    channelContentTextbox.AppendText(message.Author.Username + ": " + message.Content + "\n");
                }
                channelContentTextbox.ScrollToCaret();
            }));
        }

        private Task UpdateChannelContent(DiscordClient sender, MessageCreateEventArgs e)
        {
            return Task.Run(() => 
            { 
                this.Invoke(new MethodInvoker(delegate ()
                {
                    if (channels != null && channelsListbox.SelectedIndex != -1)
                    {
                        if (e.Channel.Id == channels[channelsListbox.SelectedIndex].Value.Id)
                        {
                            channelContentTextbox.AppendText(e.Message.Author.Username + ": " + e.Message.Content + "\n");
                            channelContentTextbox.ScrollToCaret();
                        }
                    }
                }));
            });
        }

        private void sendToChannelTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (channelsListbox.SelectedIndex != -1 && sendToChannelTextbox.Lines.Length > 0)
                {
                    var message = new Message(channels[channelsListbox.SelectedIndex].Value, sendToChannelTextbox.Lines[0]);
                    sendToChannelTextbox.Lines = new string[0];
                    Program.SendMessage(message);
                }
            }
        }

        //automatically removes old messages from the textbox
        private void RichTextbox_TextChanged(object sender, EventArgs e)
        {
            var textbox = (RichTextBox)sender;
            if(textbox.Lines.Length > 1000)
            {
                textbox.Lines = textbox.Lines.Take(800).ToArray();
            }
        }
    }
}
