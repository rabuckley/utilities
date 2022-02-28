using System.CommandLine;
using Utilities.Commands;

namespace Utilities;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var commands = new UtilitiesCommands().GetCommands();
        var cli = new UtilsRootCommand(commands);
        await cli.InvokeAsync(args).ConfigureAwait(false);
    }
}