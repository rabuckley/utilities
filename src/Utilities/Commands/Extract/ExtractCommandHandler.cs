using System.CommandLine;
using System.CommandLine.IO;
using System.IO.Abstractions;

namespace Utilities.Commands.Extract;

public class ExtractCommandHandler
{
    private readonly IFileSystem _fileSystem;
    private readonly IConsole _console;

    public ExtractCommandHandler(IFileSystem fileSystem, IConsole console)
    {
        _fileSystem = fileSystem;
        _console = console;
    }

    public Task Execute(DirectoryInfo directory)
    {
        var targetDirectory = _fileSystem.DirectoryInfo.New(directory.FullName);
        var childFiles = GetAllChildFiles(targetDirectory);

        MoveAllFiles(targetDirectory, childFiles);
        DeleteEmptySubDirectories(targetDirectory);

        return Task.CompletedTask;
    }

    private void MoveAllFiles(IFileSystemInfo directory, List<string> childFiles)
    {
        if (childFiles.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(childFiles));
        }

        foreach (var file in childFiles)
        {
            var thisFile = _fileSystem.FileInfo.New(file);
            var dest = _fileSystem.Path.Join(directory.FullName, thisFile.Name);

            if (_fileSystem.File.Exists(dest))
            {
                _console.Out.WriteLine($"[Warning] \"{dest}\" already exists. Cannot extract file \"{file}\"");
                break;
            }

            thisFile.MoveTo(dest, false);
            _console.Out.WriteLine($"[Success] Moved \"{thisFile.FullName}\" to \"{dest}\"");
        }
    }

    private List<string> GetAllChildFiles(IFileSystemInfo directory)
    {
        var immediateSubDirectories = _fileSystem.Directory.GetDirectories(directory.FullName);
        var childFiles = new List<string>();

        foreach (var subDirectory in immediateSubDirectories)
        {
            var files = _fileSystem.Directory.GetFiles(subDirectory, "*", SearchOption.AllDirectories);
            childFiles.AddRange(files);
        }

        return childFiles;
    }

    private void DeleteEmptySubDirectories(IDirectoryInfo directory)
    {
        var subDirectories = directory.GetDirectories("*", SearchOption.TopDirectoryOnly);

        foreach (var subDirectory in subDirectories)
        {
            var directoryIsNotEmpty = subDirectory.GetFiles("*", SearchOption.AllDirectories).Length > 0;

            if (directoryIsNotEmpty) break;

            subDirectory.Delete(true);
            _console.Out.WriteLine($"[Success] \"{directory.FullName}\" was empty so has been removed.");
        }
    }
}
