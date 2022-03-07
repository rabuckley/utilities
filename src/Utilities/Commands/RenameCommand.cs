using System.CommandLine;
using System.CommandLine.IO;
using System.IO.Abstractions;
using Utilities.Extensions;
using Utilities.IO;

namespace Utilities.Commands;

public class RenameCommand : Command
{
    private readonly IConsole _console;
    private readonly IFileRenamer _fileRenamer;
    private readonly IFileSystem _fileSystem;

    public RenameCommand() : this(new SystemConsole(), new FileSystem())
    {
    }

    public RenameCommand(
        IConsole console,
        IFileSystem fileSystem) :
        base("rename", "renames a file or glob to a standard format.")
    {
        _console = console;
        _fileSystem = fileSystem;
        _fileRenamer = new FileRenamer(_fileSystem);

        Argument pathArgument = new Argument<FileInfo[]>("file", "One or more files");
        AddArgument(pathArgument);

        this.SetHandler((FileInfo[] paths) => Handler(paths), pathArgument);
    }

    public new async Task Handler(IEnumerable<FileInfo> files)
    {
        var existingFiles = files.Where(file => _fileSystem.File.Exists(file.Name)).ToList();

        if (existingFiles.Count == 0) throw new FileNotFoundException();

        foreach (var file in existingFiles) await RenameFileToFormattedAsync(file.Name);
    }

    private async Task RenameFileToFormattedAsync(string fileName)
    {
        var newFileName = FormatFileName(fileName);

        if (fileName == newFileName)
        {
            _console.Out.Write($"\"{fileName}\" is already formatted correctly.");
            return;
        }

        await _fileRenamer.RenameFileAsync(fileName, newFileName);
        _console.Out.Write($"\"{fileName}\" -> \"{newFileName}\"");
    }


    private static string FormatFileName(string path)
    {
        path = new string(path.Where(c => char.IsWhiteSpace(c) || char.IsLetterOrDigit(c) || c is '.' or '-')
            .Select(char.ToLower).ToArray());
        path = path.Trim();
        path = path.Replace(' ', '-');
        path = path.ReplaceAll("..", ".");
        path = path.ReplaceAllDotsWithDashExceptFinal();
        path = path.ReplaceAll("--", "-");
        return path;
    }
}
