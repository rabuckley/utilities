using System.Collections.Concurrent;
using System.CommandLine;
using System.Diagnostics;
using System.IO.Abstractions;
using CommunityToolkit.Diagnostics;

namespace Utilities.Commands.GitUpdate;

public class GitUpdateCommandHandler
{
    private readonly IConsole _console;

    public GitUpdateCommandHandler(IConsole console)
    {
        _console = console;
    }

    public async Task ExecuteAsync(IDirectoryInfo root, CancellationToken cancellationToken = default)
    {
        var allDirectories = root.GetDirectories("*", SearchOption.AllDirectories);

        ConcurrentBag<IDirectoryInfo> gitDirectories = new();

        Parallel.ForEach(allDirectories, directory =>
        {
            if (directory.Name != ".git")
            {
                return;
            }

            if (directory.Parent is null)
            {
                ThrowHelper.ThrowInvalidOperationException(
                    "The parent directory of a .git directory was out of scope.");
            }

            gitDirectories.Add(directory.Parent!);
        });

        _console.WriteLine($"Found {gitDirectories.Count} git repositories.");

        await Parallel.ForEachAsync(gitDirectories, cancellationToken, async (gitDirectory, token) =>
        {
            var output = await PullRepositoryAsync(gitDirectory, token);

            foreach (var line in output)
            {
                _console.WriteLine(line);
            }
        });
    }

    private static async Task<List<string>> PullRepositoryAsync(
        IDirectoryInfo dir,
        CancellationToken cancellationToken = default)
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

            await p.WaitForExitAsync(cancellationToken).ConfigureAwait(true);

            var stdout = await p.StandardOutput.ReadToEndAsync(cancellationToken);
            var stderr = await p.StandardError.ReadToEndAsync(cancellationToken);

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
