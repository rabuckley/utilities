using System.CommandLine;
using System.CommandLine.IO;
using System.IO.Abstractions;

namespace Utilities.Commands;

public class ExtractCommand : Command
{
    private readonly IFileSystem _fileSystem;
    private readonly IConsole _console;
    public ExtractCommand(IConsole console, IFileSystem fileSystem) : base("extract", "gets all children of the target directory and moves all child files to that directory")
    {
        _console = console;
        _fileSystem = fileSystem;

        Argument directoryArgument = new Argument<DirectoryInfo>("directory", "the target directory");
        AddArgument(directoryArgument);

        this.SetHandler((DirectoryInfo dir) => Handler(dir), directoryArgument);
    }

    public new Task Handler(DirectoryInfo directory)
    {
        var targetDirectory = _fileSystem.DirectoryInfo.FromDirectoryName(directory.FullName);
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
            if (file is null) throw new ArgumentNullException();

            var thisFile = _fileSystem.FileInfo.FromFileName(file);
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

        Parallel.ForEach(immediateSubDirectories, subDirectory =>
        {
            var files = _fileSystem.Directory.GetFiles(subDirectory, "*", SearchOption.AllDirectories);
            childFiles.AddRange(files.Where(f => f is not null));
        });

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
