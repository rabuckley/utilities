using System.CommandLine;
using System.IO.Abstractions;
using Utilities.IO;

namespace Utilities.Commands.Rename;

public class RenameCommand : Command
{
    public RenameCommand(
        IConsole console,
        IFileSystem fileSystem,
        IFileRenamer fileRenamer) :
        base("rename", "renames a file or glob to a standard format.")
    {
        var pathArgument = new Argument<FileInfo[]>("file", "One or more files");

        Add(pathArgument);

        var handler = new RenameCommandHandler(console, fileRenamer, fileSystem);

        this.SetHandler((FileInfo[] paths) => handler.ExecuteAsync(paths), pathArgument);
    }
}
