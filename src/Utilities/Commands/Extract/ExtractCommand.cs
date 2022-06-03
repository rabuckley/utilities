using System.CommandLine;
using System.IO.Abstractions;

namespace Utilities.Commands.Extract;

public class ExtractCommand : Command
{
    public ExtractCommand(IConsole console, IFileSystem fileSystem) : base("extract",
        "gets all children of the target directory and moves all child files to that directory")
    {
        var directoryArgument = new Argument<DirectoryInfo>("directory", "the target directory");

        AddArgument(directoryArgument);

        var handler = new ExtractCommandHandler(fileSystem, console);

        this.SetHandler((DirectoryInfo dir) => handler.Execute(dir), directoryArgument);
    }
}
