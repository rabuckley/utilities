namespace Utilities.IO;

public interface IFileRenamer
{
    Task RenameFileAsync(string sourceFileName, string destFileName);
}
