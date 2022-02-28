using System.CommandLine;

namespace Utilities;

public interface IUtilitiesCommands
{
    IEnumerable<Command> GetCommands();
}