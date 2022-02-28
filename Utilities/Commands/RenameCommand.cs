using System.CommandLine;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Utilities.Commands;

public class RenameCommand : Command
{
    public RenameCommand() : base("rename", "renames a file or glob to a standard format.")
    {
        var globOption = new Option<bool>(aliases: new[] { "--glob", "-g" }, "Indicate that the passed file string is a glob");
        var pathArgument = new Argument<FileInfo[]>("file", "File or glob ");

        AddOption(globOption);
        AddArgument(pathArgument);

        this.SetHandler((FileInfo[] path, bool glob) => Handler(path, glob), pathArgument, globOption);
    }

    internal new void Handler(FileInfo[] files, bool glob)
    {
        if (glob)
        {
            var globs = files.Select(f => f.Name);
            RenameByGlob(globs);
            return;
        }

        var existingFiles = files.Where(f => f.Exists).ToList();

        if (!existingFiles.Any()) return;

        foreach (var file in existingFiles) RenameFile(file.Name);
    }


    internal static bool RenameFile(string file)
    {
        var renamedFile = FormatFilePath(file);

        if (file == renamedFile)
        {
            Console.WriteLine($"\"{file}\" is already formatted correctly.");
            return false;
        }

        File.Copy(file, renamedFile);
        File.Delete(file);
        Console.WriteLine($"\"{file} -> {renamedFile}\"");
        return true;
    }

    internal static void RenameByGlob(IEnumerable<string> globs)
    {
        Matcher matcher = new();

        globs = globs as List<string> ?? globs.ToList();

        var exclude = globs.Where(g => g[0] == '!').ToArray();
        var include = globs.Except(exclude);

        foreach (var glob in exclude) matcher.AddExclude(glob);
        foreach (var glob in include) matcher.AddInclude(glob);

        var matches = matcher.Execute(
            new DirectoryInfoWrapper(
                new DirectoryInfo(Directory.GetCurrentDirectory())));

        var changeCount = matches.Files.Select(match => RenameFile(match.Path)).Count(nameChanged => nameChanged);

        Console.WriteLine(matches.Files.Count() + " files processed. " + changeCount + " files renamed.");
    }

    private static string RemoveAll(string target, string remove, string replace)
    {
        if (!target.Contains(remove))
            return target;

        while (target.Contains(remove))
            target = target.Replace(remove, replace);

        return target;
    }

    private static string FormatFilePath(string path)
    {
        path = new string((from c in path
                           where
                               char.IsWhiteSpace(c) ||
                               char.IsLetterOrDigit(c) ||
                               c == '.' ||
                               c == '-'
                           select char.ToLower(c)).ToArray());

        // Replace spaces with -
        path = path.Replace(' ', '-');

        // Remove all double dashes
        path = RemoveAll(path, "--", "-");

        // Remove all double dots
        path = RemoveAll(path, "..", ".");

        return path;
    }
}