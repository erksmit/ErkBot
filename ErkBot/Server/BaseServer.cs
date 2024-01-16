using ErkBot.Server.Configuration;

namespace ErkBot.Server;

/// <summary>
/// The base class for servers.
/// </summary>
public abstract class BaseServer
{
    public BaseServer(BaseServerConfiguration config)
    {
        DisplayName = config.Name;
        Enabled = config.Enabled;
        LogChannelId = config.OutputChannelId;
        Status = ServerStatus.Stopped;
    }

    /// <summary>
    /// A user friendly name for the server.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// Whether the server is enabled, disabled servers should not be started.
    /// </summary>
    public bool Enabled { get; }

    /// <summary>
    /// The id of the discord channel server output can be sent to.
    /// </summary>
    public ulong LogChannelId { get; }

    private ServerStatus _status;
    public ServerStatus Status
    {
        get => _status;
        protected set
        {
            StatusChanged?.Invoke(this, value);
            _status = value;
        }
    }

    /// <summary>
    /// Fires when the status of the server changed.
    /// </summary>
    public event EventHandler<ServerStatus>? StatusChanged;

    /// <summary>
    /// Starts the server
    /// </summary>
    /// <returns>
    /// A <see cref="bool"/> indicating whether the server was started
    /// </returns>
    public abstract bool Start();

    /// <summary>
    /// Stops the server.
    /// </summary>
    /// <param name="timeOut">The amount of milliseconds to wait for the server to exit.</param>
    /// <returns>A <see cref="Task"/> that finishes when the server has exited or the <paramref name="timeOut"/> has ran out.</returns>
    public abstract Task Stop(int timeOut = 10_000);

    /// <summary>
    /// Fires when the server outputs a message.
    /// </summary>
    public event EventHandler<OutputReceivedEventArgs>? OutputReceived;

    /// <summary>
    /// Invokes the <see cref="OutputReceived"/> event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected virtual void OnMessageReceived(string message, object? sender = null)
    {
        var args = new OutputReceivedEventArgs(message);
        OutputReceived?.Invoke(sender ?? this, args);
    }
}
