using System.CommandLine;
using Utilities.Commands;

namespace Utilities;

public static class UtilitiesCli
{
    public static RootCommand CreateRootCommand()
    {
        return new UtilsRootCommand(Commands);
    }

    private static Command[] Commands { get; } =
    {
        new("test")
    };
}