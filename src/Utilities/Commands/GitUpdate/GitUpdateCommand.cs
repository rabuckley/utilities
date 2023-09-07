using System.CommandLine;
using System.IO.Abstractions;

namespace Utilities.Commands.GitUpdate;

public class GitUpdateCommand : Command
{
    public GitUpdateCommand(IConsole console, IFileSystem fileSystem) : base("git-update-all",
        "Updates all git repositories beneath the given directory")
    {
        var pathArgument = new Argument<DirectoryInfo>("dir", "The root directory");

        AddArgument(pathArgument);

        this.SetHandler(async context =>
        {
            var directory = context.ParseResult.GetValueForArgument(pathArgument);
            var cancellationToken = context.GetCancellationToken();

            var handler = new GitUpdateCommandHandler(console);
            await handler.ExecuteAsync(fileSystem.DirectoryInfo.New(directory.FullName), cancellationToken);
        });
    }
}
