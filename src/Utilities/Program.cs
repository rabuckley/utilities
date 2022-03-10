using System.CommandLine;
using System.CommandLine.IO;
using System.IO.Abstractions;
using Utilities.Commands;

IConsole console = new SystemConsole();
IFileSystem fileSystem = new FileSystem();

var utilitiesCli = new UtilsRootCommand(console, fileSystem);

await utilitiesCli.InvokeAsync(args);
