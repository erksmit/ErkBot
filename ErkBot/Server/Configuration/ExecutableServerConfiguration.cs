namespace ErkBot.Server.Configuration;
public class ExecutableServerConfiguration : BaseServerConfiguration
{
    public string StartScriptPath { get; set; }
    public string? ServerDirectory { get; set; }
}