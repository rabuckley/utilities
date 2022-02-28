using System.CommandLine;

namespace Utilities.Commands;

public class UtilsRootCommand : RootCommand
{
    public UtilsRootCommand(IUtilitiesCommands commands)
    {
        Name = "utils";
        Description = "Custom command line utilities.";

        foreach (var command in commands.GetCommands()) AddCommand(command);
    }
}