using System.CommandLine;
using System.ComponentModel.DataAnnotations;
using System.IO.Abstractions;

namespace Utilities.Commands.Flash
{
    public class FlashCommand : Command
    {
        public FlashCommand(
            IConsole console,
            IFileSystem fileSystem
        ) : base("flash", "Generates flashcards from specific markdown format")
        {
            var directoryArg = new Argument<DirectoryInfo>("dir");
            var fileNameOpt = new Option<string?>("fileName") { IsRequired = false };

            Add(directoryArg);
            Add(fileNameOpt);

            var handler = new FlashCommandHandler(console, fileSystem);

            this.SetHandler(async (dir, fileName) =>
            {
                var directory = fileSystem.DirectoryInfo.New(dir.FullName);
                await handler.ExecuteAsync(directory, fileName);
            }, directoryArg, fileNameOpt);
        }
    }
}
