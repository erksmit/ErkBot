using ErkBot.Server.Types.Minecraft.Model;
using Newtonsoft.Json;
using System.Net.Sockets;

namespace ErkBot.Server.Types.Minecraft;
internal class MinecraftPingHelper(int port)
{
    public int Port { get; private set; } = port;

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
        NetworkStreamHelper helper = new (stream);

        /*
        * Send a "Handshake" packet
        * http://wiki.vg/Server_List_Ping#Ping_Process
        */
        helper.WriteVarInt(47);
        helper.WriteString("localhost");
        helper.WriteShort(25565);
        helper.WriteVarInt(1);
        helper.Flush(0);

        /*
         * Send a "Status Request" packet
         * http://wiki.vg/Server_List_Ping#Ping_Process
         */
        helper.Flush(0);

        /*
         * If you are using a modded server then use a larger buffer to account, 
         * see link for explanation and a motd to HTML snippet
         * https://gist.github.com/csh/2480d14fbbb33b4bbae3#gistcomment-2672658
         */
        var buffer = new byte[short.MaxValue];
        // var buffer = new byte[4096];
        stream.Read(buffer, 0, buffer.Length);
        // TODO: reading from stream currently does not work
        string json = helper.ReadJsonMessage();
        var ping = JsonConvert.DeserializeObject<PingInformation>(json);
        return ping;
    }
}

