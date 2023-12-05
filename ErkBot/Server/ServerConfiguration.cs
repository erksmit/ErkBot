namespace ErkBot.Server;
public readonly record struct ServerConfiguration
{
    public ServerConfiguration(string Name, ulong OutputChannelId, string StartScriptPath , bool Enabled, ServerType Type, string ServerDirectory = null)
    {
        this.Name = Name;
        this.OutputChannelId = OutputChannelId;
        this.StartScriptPath = StartScriptPath;
        this.ServerDirectory = ServerDirectory ?? Path.GetDirectoryName(StartScriptPath);
        this.Enabled = Enabled;
        this.Type = Type;
    }

    public readonly string Name;

    public readonly ulong OutputChannelId;

    public readonly string ServerDirectory;

    public readonly string StartScriptPath;

    public readonly bool Enabled;

    public readonly ServerType Type;
}