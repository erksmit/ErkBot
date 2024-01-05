namespace ErkBot.Server.Configuration;

public class BaseServerConfiguration
{
    public BaseServerConfiguration(string name, bool enabled, ServerType type, ulong outputChannelId)
    {
        Name = name;
        Enabled = enabled;
        Type = type;
        OutputChannelId = outputChannelId;
    }

    public BaseServerConfiguration() { }

    public string Name { get; }

    public bool Enabled { get; }

    public ServerType Type { get; }

    public ulong OutputChannelId { get; }
}