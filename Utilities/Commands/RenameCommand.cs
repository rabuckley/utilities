using System.CommandLine;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Utilities.Commands
{
    public class RenameCommand : ICommand
    {
        public RenameCommand()
        {
            var globOption = new Option<bool>(aliases: new[] { "--glob", "-g" }, "Indicate that the passed file string is a glob");
            var pathArgument = new Argument<FileInfo[]>("file", "File or glob ");


            Command = new Command("rename", "renames a file or glob to a standard format.")
            {
                pathArgument,
                globOption
            };

            Command.SetHandler((FileInfo[] path, bool glob) => Handler(path, glob), pathArgument, globOption);
        }

        public Command Command { get; set; }


        internal void Handler(FileInfo[] files, bool glob)
        {
            if (glob)
            {
                var paths = new List<string>();

                foreach (var file in files)
                    paths.Add(file.Name);

                RenameByGlob(paths);
            }
            else
            {
                foreach (var file in files)
                {
                    if (file.Exists)
                        RenameFile(file.Name);
                    else
                        Console.WriteLine($"Error: \"{file.FullName}\" does not exist.");
                }
            } 
        }

        internal bool RenameFile(string file)
        {
            var path = FormatFilePath(file);

            if (file != path)
            {
                File.Copy(file, path);
                File.Delete(file);

                Console.WriteLine($"\"{file} -> {path}\"");

                return true;
            }

            Console.WriteLine($"\"{file}\" is already formatted correctly.");

            return false;
        }

        internal void RenameByGlob(IEnumerable<string> globs)
        {
            Matcher matcher = new();

            foreach (var glob in globs)
            {
                if (glob[0] == '!')
                    matcher.AddExclude(glob);
                else
                    matcher.AddInclude(glob);
            }

            var matches = matcher.Execute(
                new DirectoryInfoWrapper(
                new DirectoryInfo(Directory.GetCurrentDirectory())));

            int changeCount = 0;

            foreach (var match in matches.Files)
            {
                var nameChanged = RenameFile(match.Path);

                if (nameChanged)
                    changeCount += 1;
            }

            Console.WriteLine(matches.Files.Count() + " files processed. " + changeCount + " files renamed.");
        }

        private string RemoveAll(string target, string remove, string replace)
        {
            if (!target.Contains(remove))
                return target;

            while (target.Contains(remove))
                target = target.Replace(remove, replace);

            return target;
        }

        private string FormatFilePath(string path)
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
}
