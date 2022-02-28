using System.CommandLine;
using Utilities.Commands;

namespace Utilities;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var utilsCli = new UtilsRootCommand()
        {
            new RenameCommand()
        };

        await utilsCli.InvokeAsync(args).ConfigureAwait(false);
    }
}