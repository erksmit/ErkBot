using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using log4net.Appender;
using log4net.Core;

namespace ErkBot;

public class DiscordLogger(DiscordChannel destination) : AppenderSkeleton
{
    private readonly BufferedDiscordChannel destination = new(destination);

    protected override void Append(LoggingEvent loggingEvent)
    {
        destination.QueueMessage(loggingEvent.RenderedMessage);
    }
}