﻿using DSharpPlus.Entities;
using System.Text;
using System.Text.RegularExpressions;

namespace ErkBot;

public partial class BufferedDiscordChannel
{
    public BufferedDiscordChannel(DiscordChannel channel)
    {
        this.channel = channel;
    }

    public bool HasStarted { get; private set; }

    private readonly Queue<string> pendingMessages = new();

    private readonly ManualResetEvent consumptionSignal = new(false);

    private readonly DiscordChannel channel;

    public void QueueMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;
        IEnumerable<string> lines = MatchLineBreaks().Split(message);

        if (lines.Any(l => l.Length > MaximumLength))
        {
            List<string> lineChunks = new();
            // Split lines into chunks of 2000 characters
            foreach (string line in lines)
            {
                var chunks = SplitByLength(line, MaximumLength);
                lineChunks.AddRange(chunks);
            }
            lines = lineChunks;
        }

        lock (pendingMessages)
        {
            foreach (string line in lines)
                pendingMessages.Enqueue(line);
            consumptionSignal.Set();
        }
    }

    public void Start()
    {
        if (!HasStarted)
            Task.Factory.StartNew(ConsumeMessages, TaskCreationOptions.LongRunning);
        HasStarted = true;
    }

    private async void ConsumeMessages()
    {
        while (true)
        {
            // Block until there are messages available again.
            consumptionSignal.WaitOne();

            string message;
            lock (pendingMessages)
            {
                message = TakeMessages(pendingMessages);
                if (pendingMessages.Count == 0)
                    consumptionSignal.Reset();
            }
            await channel.SendMessageAsync(message);

            // Wait so that some messages can accumulate.
            Thread.Sleep(1000);
        }
    }

    private const int MaximumLength = 1950;

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

    private static IEnumerable<string> SplitByLength(string str, int maxLength)
    {
        for (int index = 0; index < str.Length; index += maxLength)
        {
            yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
        }
    }

    [GeneratedRegex("\r\n|\r|\n")]
    private static partial Regex MatchLineBreaks();
}