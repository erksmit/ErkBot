using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using System.Text;

namespace discord_bot;

internal class HelpFormatter : BaseHelpFormatter
{
    private StringBuilder result;

    public HelpFormatter(CommandContext ctx) : base(ctx)
    {
        this.result = new StringBuilder("```Commands:\n");
    }

    public override BaseHelpFormatter WithCommand(Command command)
    {
        result.Append($"\t{command.Name}\n");
        if (!(command is CommandGroup))
        {
            AddCommand(command);
        }
        return this;
    }

    public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
    {
        foreach (var command in subcommands)
        {
            AddCommand(command);
        }
        return this;
    }

    private void AddCommand(Command command)
    {
        result.Append($"\t{command.Name}:");
        if (command.Description != null)
            result.Append($" {command.Description}");
        result.Append($"\n\t\tusage: { command.Name}");
        var args = command.Overloads.First().Arguments;
        if (args.Count > 0)
        {
            result.Append(" <");
            var argStrings = new string[args.Count];
            for (int i = 0; i < args.Count; i++)
            {
                var arg = args[i];
                argStrings[i] = string.Empty;
                if (arg.IsOptional)
                    argStrings[i] += "?";
                argStrings[i] += arg.Name;
            }
            var argsString = string.Join(", ", argStrings);
            result.Append($"{argsString}>");
        }
        result.Append("\n");
    }

    public override CommandHelpMessage Build()
    {
        result.Append("```");
        return new CommandHelpMessage(result.ToString());
    }
}