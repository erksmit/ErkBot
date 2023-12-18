using Newtonsoft.Json;

namespace ErkBot.Server.Configuration;

public interface IServerConfiguration
{
    public string Name { get; }

    public bool Enabled { get; }

    public ServerType Type { get; }

    public ulong OutputChannelId { get; }
}