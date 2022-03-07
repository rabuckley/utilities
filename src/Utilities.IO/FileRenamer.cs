using System.IO.Abstractions;

namespace Utilities.IO;

public class FileRenamer : IFileRenamer
{
    private readonly IFileSystem _fileSystem;
    public FileRenamer(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }
    public async Task RenameFileAsync(string sourceFileName, string destFileName)
    {
        await CopyFileAsync(sourceFileName, destFileName);
        _fileSystem.File.Delete(sourceFileName);

    }
    private async Task CopyFileAsync(string sourceFile, string destinationFile, CancellationToken cancellationToken = default)
    {
        const FileOptions fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
        const int bufferSize = 65536;

        await using var sourceStream =
            _fileSystem.FileStream.Create(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions);

        await using var destinationStream =
            _fileSystem.FileStream.Create(destinationFile, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, fileOptions);

        await sourceStream.CopyToAsync(destinationStream, bufferSize, cancellationToken)
            .ConfigureAwait(continueOnCapturedContext: false);
    }
}
