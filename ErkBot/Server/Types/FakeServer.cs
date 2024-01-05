using DSharpPlus;
using ErkBot.Server.Configuration;
using Timer = System.Timers.Timer;

namespace ErkBot.Server.Types;
internal class FakeServer : BaseServer
{
    public FakeServer(DiscordClient client, BaseServerConfiguration config) : base(client, config)
    {
        timer = new Timer();
        timer.Elapsed += SendData;
        timer.Interval = 100;
    }

    private Timer timer;

    public override async Task Start()
    {
        await base.Start();
        timer.Start();
    }

    private void SendData(object? sender, EventArgs args)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var message = new string(Enumerable.Repeat(chars, Random.Shared.Next(1, 5))
            .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
        OnMessageReceived(this, new ServerMessageReceivedEventArgs(message));
    }

    public override Task Stop(int timeOut = 10000)
    {
        timer.Stop();
        return Task.CompletedTask;
    }
}
