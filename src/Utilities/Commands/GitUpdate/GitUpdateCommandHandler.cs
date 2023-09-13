using System.Collections.Concurrent;
using System.CommandLine;
using System.Diagnostics;

namespace Utilities.Commands.GitUpdate;

public class GitUpdateCommandHandler
{
    private const string GitDir = ".git";

    public async Task ExecuteAsync(DirectoryInfo root, CancellationToken cancellationToken = default)
    {
        ConcurrentBag<DirectoryInfo> gitDirectories = new();

        Parallel.ForEach(root.EnumerateDirectories(), directory =>
        {
            if (!Directory.Exists(Path.Join(directory.FullName, GitDir)))
            {
                return;
            }

            gitDirectories.Add(directory);
        });

        Console.WriteLine($"Found {gitDirectories.Count} git repositories.");

        var writeLock = new SemaphoreSlim(1, 1);

        await Parallel.ForEachAsync(gitDirectories, cancellationToken, async (gitDirectory, token) =>
        {
            var output = await PullRepositoryAsync(gitDirectory, token);

            await writeLock.WaitAsync(token);

            try
            {
                foreach (var line in output)
                {
                    Console.WriteLine(line);
                }
            }
            finally
            {
                writeLock.Release();
            }
        });
    }

    private static async Task<List<string>> PullRepositoryAsync(
        DirectoryInfo dir,
        CancellationToken cancellationToken = default)
    {
        var output = new List<string> { $"Updating '{dir.Name}'" };

        try
        {
            var gitPull = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = "pull",
                WorkingDirectory = dir.FullName,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            using var p = Process.Start(gitPull);

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
