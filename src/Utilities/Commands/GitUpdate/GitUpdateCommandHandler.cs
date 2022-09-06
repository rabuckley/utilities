using System.CommandLine;
using System.IO.Abstractions;
using LibGit2Sharp;
using Microsoft.Alm.Authentication;

namespace Utilities.Commands.GitUpdate
{
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

            var gitDirectories = (allDirectories.Where(directory => directory.Name == ".git")
                .Select(directory => directory.Parent)).ToList();

            var credentials = GetCredentials();

            var pullOptions = new PullOptions()
            {
                FetchOptions = new FetchOptions()
                {
                    CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
                    {
                        Username = credentials.Username,
                        Password = credentials.Password
                    },
                },
                MergeOptions = new MergeOptions()
                {
                    FastForwardStrategy = FastForwardStrategy.Default,
                    FileConflictStrategy = CheckoutFileConflictStrategy.Theirs,
                },
            };

            var identity = new Identity(credentials.Username, credentials.Username);

            foreach (var gitDirectory in gitDirectories)
            {
                _console.WriteLine($"Updating {gitDirectory.FullName}");
                var repository = new Repository(gitDirectory.FullName);

                try
                {
                    LibGit2Sharp.Commands.Checkout(repository, repository.Head);
                    LibGit2Sharp.Commands.Pull(repository, new Signature(identity, DateTimeOffset.Now), pullOptions);
                }
                catch (LibGit2SharpException e)
                {
                    _console.WriteLine($"Error: {e.Message}");
                }
            }
        }

        private static Credential GetCredentials()
        {
            var secrets = new SecretStore("git");
            var authentication = new BasicAuthentication(secrets);
            var credentials = authentication.GetCredentials(new TargetUri("https://github.com"));
            return credentials;
        }
    }
}
