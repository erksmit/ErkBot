using ErkBot.Server;
using ErkBot.Server.Configuration;

public readonly record struct FakeServerConfiguration(string Name, bool Enabled, ServerType Type, ulong OutputChannelId) : IServerConfiguration;