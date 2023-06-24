using System.CommandLine;
using System.IO.Abstractions;
using System.Net.Cache;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Sources;

namespace Utilities.Commands.Flash;

public class FlashCommandHandler
{
    private readonly IConsole _console;
    private readonly IFileSystem _fileSystem;
    private readonly Regex _flashMatcher;

    public FlashCommandHandler(IConsole console, IFileSystem fileSystem)
    {
        _console = console;
        _fileSystem = fileSystem;
        _flashMatcher = new Regex(@"^\*\*(.*?)\*\* - (.*?)$", RegexOptions.Multiline);
    }

    public async Task ExecuteAsync(IDirectoryInfo directory, string? fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            fileName = directory.FullName + ".txt";
        }

        var directoryNames = _fileSystem.Directory.GetDirectories(directory.FullName, "*", SearchOption.AllDirectories);

        var directories = directoryNames.Select(n => _fileSystem.DirectoryInfo.New(n)).ToList();

        directories.Add(directory);

        var flashes = await GetFlashcards(directories);

        await using var sw = new StreamWriter(fileName);
        foreach (var flash in flashes)
        {
            await sw.WriteLineAsync($"{flash.Term};{flash.Definition}");
        }
        _console.WriteLine($"{flashes.Count} flashcards successfully written to {fileName}");
    }

    private async Task<List<Flashcard>> GetFlashcards(List<IDirectoryInfo> subDirectories)
    {
        var flashes = new List<Flashcard>();

        foreach (var file in subDirectories.Select(dir => dir.GetFiles().Where(f => f.Extension == ".md"))
                     .SelectMany(files => files))
        {
            using var sr = new StreamReader(file.FullName);
            var content = await sr.ReadToEndAsync();
            var matches = _flashMatcher.Matches(content);

            if (!matches.Any())
            {
                continue;
            }

            foreach (Match match in matches)
            {
                flashes.Add(new Flashcard(match.Groups[1].Value, match.Groups[2].Value));
            }
        }

        return flashes;
    }
}

public record struct Flashcard(string Term, string Definition);
