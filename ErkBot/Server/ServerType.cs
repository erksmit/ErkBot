using System.Runtime.Serialization;

namespace ErkBot.Server;

[DataContract]
public enum ServerType
{
    [EnumMember(Value = "minecraft")]
    Minecraft,

    [EnumMember(Value = "genericExecutable")]
    Executable,

    [EnumMember(Value = "fake")]
    Fake
}
