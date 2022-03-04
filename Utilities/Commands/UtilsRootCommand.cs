using System.CommandLine;

namespace Utilities.Commands;

public sealed class UtilsRootCommand : RootCommand
{
    public UtilsRootCommand(IEnumerable<Command> commands)
    {
        Name = "utils";
        Description = "Custom command line utilities.";
        foreach (var command in commands) AddCommand(command);
    }
}