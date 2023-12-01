using System.Runtime.Serialization;

namespace ErkBot.Server;

[DataContract]
public enum ServerType
{
    [EnumMember(Value = "minecraft")]
    Minecraft
}
