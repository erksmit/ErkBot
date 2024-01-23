using ErkBot.Server.Types.Minecraft.Model;
using Newtonsoft.Json;
using System.Net.Sockets;

namespace ErkBot.Server.Types.Minecraft;
internal class MinecraftPingHelper(int port)
{
    public int Port { get; } = port;

    private const string Host = "localhost";

    public async Task<PingInformation?> GetPingInformation()
    {
        TcpClient client = new();
        try
        {
            await client.ConnectAsync(Host, Port);
        }
        catch
        {
            return null;
        }
        // return if connection failed
        if (client.Connected == false)
            return null;

        var stream = client.GetStream();

        // send a handshake packet
        var handshake = new PacketWriter(0x00);
        handshake.WriteVarInt(47); // protocol version (47 is for minecraft 1.8 but anything above 1.7 is fine)
        handshake.WriteString("localhost");
        handshake.WriteUShort((ushort)Port);
        handshake.WriteVarInt(1); // next state: status
        handshake.Send(stream);

        // send a status request packet, this contains no data
        var statusPacket = new PacketWriter(0x00);
        statusPacket.Send(stream);

        // now we can read the status from the stream
        var reader = new PacketReader(stream);
        reader.ReadVarInt(); // read and discard the complete packet length
        reader.ReadVarInt(); // read and discard the packet id
        int length = reader.ReadVarInt();
        string data = reader.ReadString(length);

        client.Close();
        var pingInfo = JsonConvert.DeserializeObject<PingInformation>(data);
        return pingInfo;
    }
}