namespace Utilities.Helpers
{
    internal interface IFileRenamer
    {
        Task<bool> RenameFileAsync(string fileName, string newFileName)
    }
}
