using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Utilities.Commands;

namespace Utilities;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        var cli = new UtilsRootCommand(new UtilitiesCommands());
        await cli.InvokeAsync(args).ConfigureAwait(false);
    }
}