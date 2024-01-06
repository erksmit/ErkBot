namespace ErkBot.Server.Configuration;

public class BaseServerConfiguration
{
    public string Name { get; set; }

    public bool Enabled { get; set; }

    public ServerType Type { get; set; }

    public ulong OutputChannelId { get; set; }
}