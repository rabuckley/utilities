using System.CommandLine;
using Utilities.Commands;

namespace Utilities;

public class UtilitiesCommands : IUtilitiesCommands
{
    public IEnumerable<Command> GetCommands()
    {
        return new List<Command>()
        {
            new RenameCommand()
        };
    }
}