﻿using System.CommandLine;
using System.IO.Abstractions;

namespace Utilities.Commands.GitUpdate
{
    public class GitUpdateCommand : Command
    {
        public GitUpdateCommand(IConsole console, IFileSystem fileSystem) : base("git-update-all",
        "Updates all git repositories beneath the given directory")
        {
            var pathArgument = new Argument<DirectoryInfo>("dir", "The root directory");

            AddArgument(pathArgument);

            this.SetHandler(async directory =>
            { var handler = new GitUpdateCommandHandler(console);
              await handler.ExecuteAsync(fileSystem.DirectoryInfo.FromDirectoryName(directory.FullName)); },
            pathArgument);
        }
    }
}