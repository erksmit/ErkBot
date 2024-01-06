using System.IO;
using System;
using System.Net.Sockets;
using System.Text;

namespace ErkBot.Server.Types.Minecraft;
internal class NetworkStreamHelper
{
    private NetworkStream stream;

    private List<byte> buffer;

    private int offset;

    public NetworkStreamHelper(NetworkStream stream)
    {
        this.stream = stream;
     
        buffer = new List<byte>();
    }

    public string ReadJsonMessage()
    {
        var buffer = new byte[Int16.MaxValue];
        // var buffer = new byte[4096];
        stream.Read(buffer, 0, buffer.Length);

        var length = ReadVarInt(buffer);
        var packet = ReadVarInt(buffer);
        var jsonLength = ReadVarInt(buffer);
        return ReadString(buffer, jsonLength);
    }

    internal byte ReadByte(byte[] buffer)
    {
        var b = buffer[offset];
        offset += 1;
        return b;
    }

    internal byte[] Read(byte[] buffer, int length)
    {
        var data = new byte[length];
        Array.Copy(buffer, offset, data, 0, length);
        offset += length;
        return data;
    }

    internal int ReadVarInt(byte[] buffer)
    {
        var value = 0;
        var size = 0;
        int b;
        while (((b = ReadByte(buffer)) & 0x80) == 0x80)
        {
            value |= (b & 0x7F) << (size++ * 7);
            if (size > 5)
            {
                throw new IOException("This VarInt is an imposter!");
            }
        }
        return value | ((b & 0x7F) << (size * 7));
    }

    internal string ReadString(byte[] buffer, int length)
    {
        var data = Read(buffer, length);
        return Encoding.UTF8.GetString(data);
    }

    internal void WriteVarInt(int value)
    {
        while ((value & 128) != 0)
        {
            buffer.Add((byte)(value & 127 | 128));
            value = (int)((uint)value) >> 7;
        }
        buffer.Add((byte)value);
    }

    internal void WriteShort(short value)
    {
        buffer.AddRange(BitConverter.GetBytes(value));
    }

    internal void WriteString(string data)
    {
        var dataBytes = Encoding.UTF8.GetBytes(data);
        WriteVarInt(dataBytes.Length);
        buffer.AddRange(dataBytes);
    }

    internal void Write(byte b)
    {
        stream.WriteByte(b);
    }

    internal void Flush(int id = -1)
    {
        var arrayBuffer = buffer.ToArray();
        buffer.Clear();

        var add = 0;
        var packetData = new[] { (byte)0x00 };
        if (id >= 0)
        {
            WriteVarInt(id);
            packetData = buffer.ToArray();
            add = packetData.Length;
            buffer.Clear();
        }

        WriteVarInt(arrayBuffer.Length + add);
        var bufferLength = buffer.ToArray();
        buffer.Clear();

        stream.Write(bufferLength, 0, bufferLength.Length);
        stream.Write(packetData, 0, packetData.Length);
        stream.Write(arrayBuffer, 0, arrayBuffer.Length);
    }
}
