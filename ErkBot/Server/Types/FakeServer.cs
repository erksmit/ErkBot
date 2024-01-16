using ErkBot.Server.Configuration;
using Timer = System.Timers.Timer;

namespace ErkBot.Server.Types;

/// <summary>
/// A fake server that outputs random message on an interval.
/// </summary>
internal class FakeServer : BaseServer
{
    public FakeServer(BaseServerConfiguration config) : base(config)
    {
        timer = new Timer();
        timer.Elapsed += SendData;
        timer.Interval = 2000;
    }

    private readonly Timer timer;

    public override bool Start()
    {
        timer.Start();
        return true;
    }

    private void SendData(object? sender, EventArgs args)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var message = new string(Enumerable.Repeat(chars, Random.Shared.Next(1, 5))
            .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
        OnMessageReceived(message);
    }

    public override Task Stop(int timeOut = 10000)
    {
        timer.Stop();
        return Task.CompletedTask;
    }
}
