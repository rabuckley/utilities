namespace Utilities.IO
{
    public interface IFileRenamer
    {
        Task RenameFileAsync(string fileName, string newFileName);
    }
}
