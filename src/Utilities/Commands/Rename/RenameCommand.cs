using System.CommandLine;
using System.CommandLine.IO;
using System.IO.Abstractions;
using Humanizer;
using Utilities.Commands.Rename;
using Utilities.Extensions;
using Utilities.IO;

namespace Utilities.Commands;

public class RenameCommand : Command
{
    public RenameCommand(
        IConsole console,
        IFileSystem fileSystem,
        IFileRenamer fileRenamer) :
        base("rename", "renames a file or glob to a standard format.")
    {
        Argument pathArgument = new Argument<FileInfo[]>("file", "One or more files");
        AddArgument(pathArgument);

        var handler = new RenameCommandHandler(console, fileRenamer, fileSystem);

        this.SetHandler((FileInfo[] paths) => handler.ExecuteAsync(paths), pathArgument);
    }
}
