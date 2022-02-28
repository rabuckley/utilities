using System.CommandLine;

namespace Utilities.Commands;

public class UtilsRootCommand : RootCommand
{
    public UtilsRootCommand()
    {
        Name = "utils";
        Description = "Custom command line utilities.";
    }
}