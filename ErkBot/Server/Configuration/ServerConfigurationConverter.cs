using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ErkBot.Server.Configuration;
internal class ServerConfigurationConverter : JsonConverter<IServerConfiguration>
{
    public override bool CanWrite => false;

    public override IServerConfiguration? ReadJson(JsonReader reader, Type objectType, IServerConfiguration? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);

        JToken token = jo["type"] ?? throw new ConfigurationException("One or more servers do not have a type configured");
        var type = serializer.Deserialize<ServerType>(token.CreateReader());

        var joReader = jo.CreateReader();
        IServerConfiguration config;
        switch (type)
        {
            case ServerType.Executable:
            case ServerType.Minecraft:
                config = new ExecutableServerConfiguration();
                break;
            case ServerType.Fake:
                config = new FakeServerConfiguration();
                break;
            default:
                throw new ConfigurationException("One or more servers has an invalid type configured");
        }
        serializer.Populate(joReader, config); 
        return config;
    }
    public override void WriteJson(JsonWriter writer, IServerConfiguration? value, JsonSerializer serializer)
    {
        throw new NotImplementedException("This converter is read only");
    }
}

