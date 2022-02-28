using System.CommandLine;
using Utilities.Commands;

namespace Utilities;

public class UtilitiesCommands
{
    public IEnumerable<Command> GetCommands()
    {
        return new List<Command>()
        {
            new RenameCommand()
        };
    }
}