namespace Utilities.Helpers
{
    public static class FileHelper
    {
        public static async Task CopyFileAsync(string sourceFile, string destinationFile, CancellationToken cancellationToken = default)
        {
            const FileOptions fileOptions = FileOptions.Asynchronous | FileOptions.SequentialScan;
            const int bufferSize = 65536;

            await using var sourceStream =
                  new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, fileOptions);

            await using var destinationStream =
                  new FileStream(destinationFile, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, fileOptions);

            await sourceStream.CopyToAsync(destinationStream, bufferSize, cancellationToken)
                                       .ConfigureAwait(continueOnCapturedContext: false);
        }

        public static async Task RenameFileAsync(string sourceFileName, string destFileName)
        {
            await CopyFileAsync(sourceFileName, destFileName);
            File.Delete(sourceFileName);
        }
    }
}

