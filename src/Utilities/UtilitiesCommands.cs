using System.CommandLine;
using Utilities.Commands;

namespace Utilities;

public static class UtilitiesCli
{
    public static RootCommand CreateApp()
    {
        var rootCommand = new UtilsRootCommand();

        rootCommand.AddCommand(new RenameCommand());

        return rootCommand;
    }
}