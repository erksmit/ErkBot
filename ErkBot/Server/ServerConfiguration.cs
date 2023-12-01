namespace ErkBot.Server;
public record ServerConfiguration(string Name, ulong OutputChannelId, bool Enabled, ServerType Type);