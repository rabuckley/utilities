using System.CommandLine;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Utilities.Extensions;
using Utilities.Helpers;
using Utilities.IO;

namespace Utilities.Commands;

public class RenameCommand : Command
{
    public RenameCommand() : base("rename", "renames a file or glob to a standard format.")
    {
        var globOption = new Option<bool>(new[] { "--glob", "-g" }, "Indicate that the passed file string is a glob");
        var pathArgument = new Argument<FileInfo[]>("file", "File or glob ");

        AddOption(globOption);
        AddArgument(pathArgument);

        this.SetHandler((FileInfo[] path, bool glob) => Handler(path, glob), pathArgument, globOption);
    }


    private new static async Task Handler(IEnumerable<FileInfo> files, bool glob)
    {
        if (glob)
        {
            var globs = files.Select(f => f.Name);
            RenameByGlob(globs);
            return;
        }

        var existingFiles = files.Where(f => f.Exists).ToList();

        if (!existingFiles.Any()) return;

        foreach (var file in existingFiles) await RenameFileAsync(file.Name);
    }


    private static void RenameByGlob(IEnumerable<string> globs)
    {
        var globMatcher = GlobHelper.CreateGlobMatcher(globs);

        var currentDirectory = new DirectoryInfoWrapper(new DirectoryInfo(Directory.GetCurrentDirectory()));

        var matchingFiles = globMatcher.Execute(currentDirectory);

        var changedFiles = matchingFiles.Files.Select(file => RenameFileAsync(file.Path));

        Console.WriteLine(matchingFiles.Files.Count() + " files processed. " + changedFiles.Count() + " files renamed.");
    }


    private static async Task<bool> RenameFileAsync(string fileName)
    {
        var newFileName = FormatFileName(fileName);

        if (fileName == newFileName)
        {
            Console.WriteLine($"\"{fileName}\" is already formatted correctly.");
            return false;
        }

        await FileHelper.RenameFileAsync(fileName, newFileName);

        Console.WriteLine($"\"{fileName}\" -> \"{newFileName}\"");

        return true;
    }


    private static string FormatFileName(string path)
    {
        path = new string(path.Where(c => Char.IsWhiteSpace(c) || Char.IsLetterOrDigit(c) || c is '.' or '-').Select(Char.ToLower).ToArray());
        path = path.Trim();
        path = path.Replace(' ', '-');
        path = path.RemoveAll("--", "-");
        path = path.RemoveAll("..", ".");
        return path;
    }
}