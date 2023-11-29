
namespace discord_bot
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.systemOutputTextbox = new System.Windows.Forms.RichTextBox();
            this.systemOutputLabel = new System.Windows.Forms.Label();
            this.serverOutputTextbox = new System.Windows.Forms.RichTextBox();
            this.serverOutputsLabel = new System.Windows.Forms.Label();
            this.pendingMessagesLabel = new System.Windows.Forms.Label();
            this.discordServersListbox = new System.Windows.Forms.ListBox();
            this.channelsListbox = new System.Windows.Forms.ListBox();
            this.sendToChannelTextbox = new System.Windows.Forms.TextBox();
            this.channelContentTextbox = new System.Windows.Forms.RichTextBox();
            this.serversLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.serverListbox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // systemOutputTextbox
            // 
            this.systemOutputTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.systemOutputTextbox.Cursor = System.Windows.Forms.Cursors.Default;
            this.systemOutputTextbox.Location = new System.Drawing.Point(11, 23);
            this.systemOutputTextbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.systemOutputTextbox.Name = "systemOutputTextbox";
            this.systemOutputTextbox.ReadOnly = true;
            this.systemOutputTextbox.Size = new System.Drawing.Size(468, 404);
            this.systemOutputTextbox.TabIndex = 0;
            this.systemOutputTextbox.Text = "";
            this.systemOutputTextbox.TextChanged += new System.EventHandler(this.RichTextbox_TextChanged);
            // 
            // systemOutputLabel
            // 
            this.systemOutputLabel.AutoSize = true;
            this.systemOutputLabel.Location = new System.Drawing.Point(9, 8);
            this.systemOutputLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.systemOutputLabel.Name = "systemOutputLabel";
            this.systemOutputLabel.Size = new System.Drawing.Size(72, 13);
            this.systemOutputLabel.TabIndex = 1;
            this.systemOutputLabel.Text = "system output";
            // 
            // serverOutputTextbox
            // 
            this.serverOutputTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.serverOutputTextbox.Cursor = System.Windows.Forms.Cursors.Default;
            this.serverOutputTextbox.Location = new System.Drawing.Point(564, 23);
            this.serverOutputTextbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.serverOutputTextbox.Name = "serverOutputTextbox";
            this.serverOutputTextbox.ReadOnly = true;
            this.serverOutputTextbox.Size = new System.Drawing.Size(548, 414);
            this.serverOutputTextbox.TabIndex = 2;
            this.serverOutputTextbox.Text = "";
            this.serverOutputTextbox.TextChanged += new System.EventHandler(this.RichTextbox_TextChanged);
            // 
            // serverOutputsLabel
            // 
            this.serverOutputsLabel.AutoSize = true;
            this.serverOutputsLabel.Location = new System.Drawing.Point(561, 8);
            this.serverOutputsLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.serverOutputsLabel.Name = "serverOutputsLabel";
            this.serverOutputsLabel.Size = new System.Drawing.Size(77, 13);
            this.serverOutputsLabel.TabIndex = 3;
            this.serverOutputsLabel.Text = "server outputs:";
            // 
            // pendingMessagesLabel
            // 
            this.pendingMessagesLabel.AutoSize = true;
            this.pendingMessagesLabel.Location = new System.Drawing.Point(11, 428);
            this.pendingMessagesLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.pendingMessagesLabel.Name = "pendingMessagesLabel";
            this.pendingMessagesLabel.Size = new System.Drawing.Size(107, 13);
            this.pendingMessagesLabel.TabIndex = 5;
            this.pendingMessagesLabel.Text = "pending messages: 0";
            // 
            // discordServersListbox
            // 
            this.discordServersListbox.FormattingEnabled = true;
            this.discordServersListbox.Location = new System.Drawing.Point(1123, 23);
            this.discordServersListbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.discordServersListbox.Name = "discordServersListbox";
            this.discordServersListbox.Size = new System.Drawing.Size(160, 121);
            this.discordServersListbox.TabIndex = 6;
            this.discordServersListbox.SelectedIndexChanged += new System.EventHandler(this.serversListbox_SelectedIndexChanged);
            // 
            // channelsListbox
            // 
            this.channelsListbox.FormattingEnabled = true;
            this.channelsListbox.Location = new System.Drawing.Point(1123, 160);
            this.channelsListbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.channelsListbox.Name = "channelsListbox";
            this.channelsListbox.Size = new System.Drawing.Size(160, 277);
            this.channelsListbox.TabIndex = 7;
            this.channelsListbox.SelectedIndexChanged += new System.EventHandler(this.channelsListbox_SelectedIndexChanged);
            // 
            // sendToChannelTextbox
            // 
            this.sendToChannelTextbox.Location = new System.Drawing.Point(1286, 419);
            this.sendToChannelTextbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.sendToChannelTextbox.Name = "sendToChannelTextbox";
            this.sendToChannelTextbox.Size = new System.Drawing.Size(303, 20);
            this.sendToChannelTextbox.TabIndex = 8;
            this.sendToChannelTextbox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.sendToChannelTextbox_KeyUp);
            // 
            // channelContentTextbox
            // 
            this.channelContentTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.channelContentTextbox.Cursor = System.Windows.Forms.Cursors.Default;
            this.channelContentTextbox.Location = new System.Drawing.Point(1286, 23);
            this.channelContentTextbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.channelContentTextbox.Name = "channelContentTextbox";
            this.channelContentTextbox.ReadOnly = true;
            this.channelContentTextbox.Size = new System.Drawing.Size(303, 393);
            this.channelContentTextbox.TabIndex = 9;
            this.channelContentTextbox.Text = "";
            this.channelContentTextbox.TextChanged += new System.EventHandler(this.RichTextbox_TextChanged);
            // 
            // serversLabel
            // 
            this.serversLabel.AutoSize = true;
            this.serversLabel.Location = new System.Drawing.Point(1121, 8);
            this.serversLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.serversLabel.Name = "serversLabel";
            this.serversLabel.Size = new System.Drawing.Size(78, 13);
            this.serversLabel.TabIndex = 10;
            this.serversLabel.Text = "discord servers";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1121, 145);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "channels";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(479, 8);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "servers";
            // 
            // serverListbox
            // 
            this.serverListbox.FormattingEnabled = true;
            this.serverListbox.Location = new System.Drawing.Point(483, 23);
            this.serverListbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.serverListbox.Name = "serverListbox";
            this.serverListbox.Size = new System.Drawing.Size(79, 95);
            this.serverListbox.TabIndex = 13;
            this.serverListbox.SelectedIndexChanged += new System.EventHandler(this.serverListbox_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1623, 445);
            this.Controls.Add(this.serverListbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.serversLabel);
            this.Controls.Add(this.channelContentTextbox);
            this.Controls.Add(this.sendToChannelTextbox);
            this.Controls.Add(this.channelsListbox);
            this.Controls.Add(this.discordServersListbox);
            this.Controls.Add(this.pendingMessagesLabel);
            this.Controls.Add(this.serverOutputsLabel);
            this.Controls.Add(this.serverOutputTextbox);
            this.Controls.Add(this.systemOutputLabel);
            this.Controls.Add(this.systemOutputTextbox);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.RichTextBox systemOutputTextbox;
        private System.Windows.Forms.Label systemOutputLabel;
        public System.Windows.Forms.RichTextBox serverOutputTextbox;
        private System.Windows.Forms.Label serverOutputsLabel;
        private System.Windows.Forms.Label pendingMessagesLabel;
        private System.Windows.Forms.ListBox discordServersListbox;
        private System.Windows.Forms.ListBox channelsListbox;
        private System.Windows.Forms.TextBox sendToChannelTextbox;
        private System.Windows.Forms.RichTextBox channelContentTextbox;
        private System.Windows.Forms.Label serversLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox serverListbox;
    }
}