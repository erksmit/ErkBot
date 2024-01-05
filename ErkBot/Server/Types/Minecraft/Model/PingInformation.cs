namespace ErkBot.Server.Types.Minecraft.Model;
public readonly record struct PingInformation(
    Version Version,
    Players Players,
    Description Description,
    string Favicon,
    bool EnforcesSecureChat,
    bool PreviewsChat
);