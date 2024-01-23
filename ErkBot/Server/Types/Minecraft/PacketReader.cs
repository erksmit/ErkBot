using System.Text;

namespace ErkBot.Server.Types.Minecraft;
internal class PacketReader
{
    private Stream stream;

    public PacketReader(Stream stream)
    {
        this.stream = stream;
    }

    public int ReadVarInt()
    {
        // implementation of https://wiki.vg/Protocol#VarInt_and_VarLong
        const int SEGMENT_BITS = 0x7F;
        const int CONTINUE_BIT = 0x80;

        int value = 0;
        int position = 0;
        
        while(true)
        {
            byte currentByte = (byte)stream.ReadByte();
            value |= (currentByte & SEGMENT_BITS) << position;

            if ((currentByte & CONTINUE_BIT) == 0)
                break;
            position += 7;
            if (position >= 32)
                throw new IOException("VarInt is too big");
        }
        return value;
    }

    public string ReadString(int length)
    {
        var buffer = new byte[length];
        stream.Read(buffer, 0, length);
        string data = Encoding.UTF8.GetString(buffer);
        return data;
    }
}
