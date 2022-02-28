using System.CommandLine;
using Utilities.Commands;

namespace Utilities;

public static class UtilitiesCommands
{
    public static IEnumerable<Command> GetCommands()
    {
        return new List<Command>()
        {
            new RenameCommand()
        };
    }
}