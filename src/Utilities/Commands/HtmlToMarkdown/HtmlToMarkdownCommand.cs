using System.CommandLine;
using System.IO.Abstractions;

namespace Utilities.Commands.HtmlToMarkdown
{
    public class HtmlToMarkdownCommand : Command
    {
        public HtmlToMarkdownCommand(IConsole console, IFileSystem fileSystem) : base("html-to-markdown", "Converts HTML documents to markdown")
        {
            var inputFile = new Option<FileInfo?>("--file", "The input file")
            {
                IsRequired = false
            };
            var inputDirectory = new Option<DirectoryInfo?>("--directory", "The input directory")
            {
                IsRequired = false
            };

            AddOption(inputFile);
            AddOption(inputDirectory);

            this.SetHandler(async (context) =>
            {
                var file = context.ParseResult.GetValueForOption(inputFile);
                var dir = context.ParseResult.GetValueForOption(inputDirectory);

                if (file is null && dir is null)
                {
                    throw new ArgumentException("Either --file or --directory must be specified");
                }

                if (file is not null && dir is not null)
                {
                    throw new ArgumentException("Only one of --file or --directory can be specified");
                }

                var cancellationToken = context.GetCancellationToken();

                var handler = new HtmlToMarkdownCommandHandler(console, fileSystem);

                if (file is not null)
                {
                    await handler.HandleAsync(fileSystem.FileInfo.New(file.FullName), cancellationToken);
                }
                else
                {
                    await handler.HandleAsync(fileSystem.DirectoryInfo.New(dir!.FullName), cancellationToken);
                }
            });
        }
    }
}
