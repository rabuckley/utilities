using System.CommandLine;
using System.IO.Abstractions;
using Utilities.Commands.Extract;
using Utilities.Commands.Flash;
using Utilities.Commands.GitUpdate;
using Utilities.Commands.Rename;
using Utilities.IO;

namespace Utilities.Commands;

public sealed class UtilsRootCommand : RootCommand
{
    public override string Name { get; set; } = "utils";
    public override string? Description { get; set; } = "Custom command line utilities.";

    public UtilsRootCommand(IConsole console, IFileSystem fileSystem)
    {
        Add(new RenameCommand(console, fileSystem, new FileRenamer(fileSystem)));
        Add(new ExtractCommand(console, fileSystem));
        Add(new FlashCommand(console, fileSystem));
        Add(new GitUpdateCommand(console, fileSystem));
    }
}
