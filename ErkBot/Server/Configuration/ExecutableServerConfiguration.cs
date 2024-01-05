namespace ErkBot.Server.Configuration;
public class ExecutableServerConfiguration : BaseServerConfiguration
{
    public ExecutableServerConfiguration()
    {
    }

    public ExecutableServerConfiguration(string name, bool enabled, ServerType type, ulong outputChannelId, string startScriptPath, string? serverDirectory = null) : base(name, enabled, type, outputChannelId)
    {
        StartScriptPath = startScriptPath;
        ServerDirectory = serverDirectory;
    }

    public string StartScriptPath {  get; }
    public string? ServerDirectory { get; }
}