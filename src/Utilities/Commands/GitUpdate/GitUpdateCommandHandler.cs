using System.CommandLine;
using System.Diagnostics;
using System.IO.Abstractions;

namespace Utilities.Commands.GitUpdate;

public class GitUpdateCommandHandler
{
    private readonly IConsole _console;

    public GitUpdateCommandHandler(IConsole console)
    {
        _console = console;
    }

    public async Task ExecuteAsync(IDirectoryInfo root)
    {
        var allDirectories = root.GetDirectories("*", SearchOption.AllDirectories);

        var gitDirectories = allDirectories
                             .Where(directory => directory.Name == ".git")
                             .Select(directory => directory.Parent).ToList();

        _console.WriteLine($"Found {gitDirectories.Count} git repositories.");

        var tasks = new List<Task<List<string>>>();

        foreach (var gitDirectory in gitDirectories)
        {
            if (gitDirectory is null)
            {
                throw new InvalidOperationException("The parent directory of a .git directory was out of scope.");
            }

            var task = PullRepositoryAsync(gitDirectory);

            tasks.Add(task);
        }

        foreach (var task in tasks)
        {
            var output = await task;

            foreach (var line in output)
            {
                _console.WriteLine(line);
            }
        }
    }

    private static async Task<List<string>> PullRepositoryAsync(IDirectoryInfo dir)
    {
        var output = new List<string> { $"Updating '{dir.Name}'" };

        try
        {
            var p = Process.Start(new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "pull",
                WorkingDirectory = dir.FullName,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            });

            if (p is null)
            {
                throw new InvalidOperationException("Failed to start git process.");
            }

            await p.WaitForExitAsync().ConfigureAwait(true);

            var stdout = await p.StandardOutput.ReadToEndAsync();
            var stderr = await p.StandardError.ReadToEndAsync();

            if (stderr.Length > 0)
            {
                output.Add(stderr);
            }

            if (stdout.Length > 0)
            {
                output.Add(stdout);
            }
        }
        catch (Exception e)
        {
            output.Add($"Failed to update '{dir.Name}': {e.Message}");
        }

        return output;
    }
}
