namespace ErkBot.Server.Configuration;

public class MinecraftServerConfiguration : ExecutableServerConfiguration
{
    public MinecraftServerConfiguration() { }

    public MinecraftServerConfiguration(string name, bool enabled, ServerType type, ulong outputChannelId, string startScriptPath, int port = 25565, string? serverDirectory = null) : base(name, enabled, type, outputChannelId, startScriptPath, serverDirectory)
    {
        Port = port;
    }

    public int Port { get; }
}
