namespace ErkBot.Server.Types.Minecraft.Model;

public readonly record struct Players(
    int Max,
    int Online,
    Player[] Sample
);