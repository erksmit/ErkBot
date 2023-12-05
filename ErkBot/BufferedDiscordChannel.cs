using System.Text;

using DSharpPlus.Entities;

namespace ErkBot;

public class BufferedDiscordChannel(DiscordChannel channel)
{
    public bool hasStarted { get; private set; }

    private readonly Queue<string> pendingMessages = new();

    private readonly ManualResetEvent consumptionSignal = new(false);

    public void QueueMessage(string message)
    {
        lock (pendingMessages)
        {
            pendingMessages.Enqueue(message);
            consumptionSignal.Set();
        }
    }

    public void Start()
    {
        if (!hasStarted)
            Task.Factory.StartNew(ConsumeMessages, TaskCreationOptions.LongRunning);
        hasStarted = true;
    }

    private async void ConsumeMessages()
    {
        while (true)
        {
            string message;
            lock (pendingMessages)
            {
                if (pendingMessages.Count == 0)
                    consumptionSignal.Reset();
                message = TakeMessages(pendingMessages);
            }
            await channel.SendMessageAsync(message);

            // Wait so that some messages can accumulate.
            Thread.Sleep(1000);
            // Block until there are messages available again.
            consumptionSignal.WaitOne();
        }
    }

    private const int MaximumLength = 2000;

    private string TakeMessages(Queue<string> messages)
    {
        StringBuilder result = new();
        bool done = false;
        do
        {
            var nextMessageLength = messages.Peek().Length;
            if (result.Length + nextMessageLength <= MaximumLength)
                result.AppendLine(messages.Dequeue());
            else
                done = true;
            if (messages.Count == 0)
                done = true;
        } while (!done);
        return result.ToString();
    }
}