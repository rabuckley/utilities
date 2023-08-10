using System.CommandLine;
using System.CommandLine.IO;
using System.IO.Abstractions;
using Humanizer;
using Utilities.Extensions;
using Utilities.IO;

namespace Utilities.Commands.Rename;

public class RenameCommandHandler
{
    private readonly IConsole _console;
    private readonly IFileRenamer _fileRenamer;
    private readonly IFileSystem _fileSystem;

    public RenameCommandHandler(IConsole console, IFileRenamer fileRenamer, IFileSystem fileSystem)
    {
        _console = console;
        _fileSystem = fileSystem;
        _fileRenamer = fileRenamer;
    }

    public async Task ExecuteAsync(IEnumerable<FileInfo> files)
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
            WriteSuccess();
            _console.Out.WriteLine($"\"{fileName}\" is already formatted correctly.");
            return;
        }

        await _fileRenamer.RenameFileAsync(fileName, newFileName);

        WriteSuccess();
        _console.Out.WriteLine($"\"{fileName}\" -> \"{newFileName}\"");
    }


    private static string FormatFileName(string path)
    {
        var (name, extension) = path.SeparateBodyAndExtensionFromPath();
        name = name.Trim();
        name = name.Replace(".", " ").Kebaberize().ReplaceAll("--", "-");
        return name + extension;
    }

    private static void WriteSuccess()
    {
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write("[Success] ");
        Console.ForegroundColor = ConsoleColor.White;
    }
}