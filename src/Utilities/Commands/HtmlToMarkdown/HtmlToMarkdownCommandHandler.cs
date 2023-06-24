using System.CommandLine;
using System.IO.Abstractions;
using Pandoc;

namespace Utilities.Commands.HtmlToMarkdown
{
    public class HtmlToMarkdownCommandHandler
    {
        private readonly IConsole _console;
        private readonly IFileSystem _fileSystem;
        private readonly PandocEngine _engine;
        private readonly CommonMarkOut _markdownOptions;
        private readonly HtmlIn _htmlOptions;

        public HtmlToMarkdownCommandHandler(IConsole console, IFileSystem fileSystem)
        {
            _console = console;
            _fileSystem = fileSystem;
            _engine = new PandocEngine("C:/Users/ryan/AppData/Local/Pandoc/pandoc.exe");

            _htmlOptions = new HtmlIn();

            _markdownOptions = new CommonMarkOut
            {
                Wrap = Wrap.None
            };
        }

        public async Task HandleAsync(IFileInfo file, CancellationToken cancellationToken)
        {
            await using var htmlInStream = file.OpenRead();
            var markdownFile = _fileSystem.FileInfo.New(_fileSystem.Path.ChangeExtension(file.FullName, ".md"));
            await using var markdownOutStream = markdownFile.OpenWrite();
            _console.WriteLine($"{file.Name} => {markdownFile.Name}");
            await _engine.Convert(htmlInStream, markdownOutStream, _htmlOptions, _markdownOptions, null, cancellationToken);
        }

        public async Task HandleAsync(IDirectoryInfo dir, CancellationToken cancellationToken)
        {
            var files = dir.GetFiles("*.html", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                await HandleAsync(file, cancellationToken);
            }
        }
    }
}
