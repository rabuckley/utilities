using System.CommandLine;
using System.IO.Abstractions;
using Utilities.IO;

namespace Utilities.Commands;

public sealed class UtilsRootCommand : RootCommand
{
    public override string Name => "utils";
    public override string Description => "Custom command line utilities.";
    public UtilsRootCommand(IConsole console, IFileSystem fileSystem)
    {
        AddCommand(new RenameCommand(console, fileSystem, new FileRenamer(fileSystem)));
        AddCommand(new ExtractCommand(console, fileSystem));
    }
}
