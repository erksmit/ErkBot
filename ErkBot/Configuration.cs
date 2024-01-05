using ErkBot.Server.Configuration;
using Newtonsoft.Json;

namespace ErkBot;

public readonly record struct Configuration(
    char Prefix,
    string DiscordToken,
    ulong LogChannelId,
    BaseServerConfiguration[] Servers
)
{
    public static Configuration LoadConfiguration(string path = "appsettings.json")
    {
        var text = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<Configuration>(text, new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new ServerConfigurationConverter()
            }
        });
    }
};
