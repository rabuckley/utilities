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

        public void Execute(IDirectoryInfo root)
        {
            var allDirectories = root.GetDirectories("*", SearchOption.AllDirectories);

            var gitDirectories = allDirectories
                .Where(directory => directory.Name == ".git")
                .Select(directory => directory.Parent).ToList();

            var credentials = GetCredentials();

            var pullOptions = new PullOptions
            {
                FetchOptions = new FetchOptions
                {
                    CredentialsProvider = (url, user, cred) => new UsernamePasswordCredentials
                    {
                        Username = credentials.Username,
                        Password = credentials.Password
                    },
                },
                MergeOptions = new MergeOptions
                {
                    FastForwardStrategy = FastForwardStrategy.Default,
                    FileConflictStrategy = CheckoutFileConflictStrategy.Theirs,
                },
            };

            var identity = new Identity(credentials.Username, credentials.Username);

            var tasks = new List<Task>();

            foreach (var gitDirectory in gitDirectories)
            {
                if (gitDirectory is null)
                {
                    throw new InvalidOperationException("The parent directory of a .git directory was out of scope.");
                }

                _console.WriteLine($"Updating '{gitDirectory.Name}'");
                
                var task = Task.Run(() =>
                {
                    var repository = new Repository(gitDirectory.FullName);

                    try
                    {
                        LibGit2Sharp.Commands.Checkout(repository, repository.Head);
                        LibGit2Sharp.Commands.Pull(repository, new Signature(identity, DateTimeOffset.Now), pullOptions);
                    }
                    catch (LibGit2SharpException e)
                    {
                        _console.WriteLine($"[{gitDirectory.Name}] Error: '{e.Message}'");
                    }
                });
                
                tasks.Add(task);
            }
            
            Task.WaitAll(tasks.ToArray());
        }

        private static Credential GetCredentials()
        {
            var authentication = new BasicAuthentication(new SecretStore("git"));
            return authentication.GetCredentials(new TargetUri("https://github.com"));
        }
    }
}
