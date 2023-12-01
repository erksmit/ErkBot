namespace ErkBot.Server.Minecraft;
public record MinecraftServerConfiguration(
    string ServerDirectory, 
    string StartScriptPath, 
    string Name, 
    ulong OutputChannelId, 
    bool Enabled,
    ServerType Type
) : ServerConfiguration(Name, OutputChannelId, Enabled, Type);