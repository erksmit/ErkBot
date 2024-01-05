using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ErkBot.Server.Configuration;
internal class ServerConfigurationConverter : JsonConverter<BaseServerConfiguration>
{
    public override bool CanWrite => false;

    public override BaseServerConfiguration? ReadJson(JsonReader reader, Type objectType, BaseServerConfiguration? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);

        JToken token = jo["type"] ?? throw new ConfigurationException("One or more servers do not have a type configured");
        var type = serializer.Deserialize<ServerType>(token.CreateReader());

        var joReader = jo.CreateReader();
        BaseServerConfiguration config;
        switch (type)
        {
            case ServerType.Executable:
                config = new ExecutableServerConfiguration();
                break;
            case ServerType.Minecraft:
                config = new MinecraftServerConfiguration();
                break;
            case ServerType.Fake:
                config = new BaseServerConfiguration();
                break;
            default:
                throw new ConfigurationException("One or more servers has an invalid type configured");
        }
        serializer.Populate(joReader, config);
        return config;
    }
    public override void WriteJson(JsonWriter writer, BaseServerConfiguration? value, JsonSerializer serializer)
    {
        throw new NotImplementedException("This converter is read only");
    }
}

