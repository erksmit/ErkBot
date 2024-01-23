using System.Text;

namespace ErkBot.Server.Types.Minecraft;
internal class PacketWriter
{
    private List<byte> byteList;

    public PacketWriter(byte packetId)
    {
        byteList = new List<byte>();
        WriteVarInt(packetId);
    }

    public void WriteVarInt(int value)
    {
        while ((value & 128) != 0)
        {
            byteList.Add((byte)(value & 127 | 128));
            value = (int)((uint)value) >> 7;
        }
        byteList.Add((byte)value);
    }

    public void WriteUShort(ushort value)
    {
        var bytes = BitConverter.GetBytes(value);
        // Shorts must be big endian so we must reverse the bytes
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        byteList.AddRange(bytes);
    }

    public void WriteString(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        // Write the length of the string in bytes to the message
        WriteVarInt(bytes.Length);
        byteList.AddRange(bytes);
    }

    public void WriteInt(int value)
    {
        var bytes = BitConverter.GetBytes(value);
        // Ints must be big endian so we must reverse the bytes
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        byteList.AddRange(bytes);
    }


    public void Send(Stream stream)
    {
        // Prepend the length of the message to the byte list
        var byteListCopy = byteList;
        byteList = new List<byte>();
        WriteVarInt(byteListCopy.Count);
        byteList.AddRange(byteListCopy);

        var byteArray = byteList.ToArray();
        stream.Write(byteArray, 0, byteArray.Length);
        stream.Flush();
    }
}
