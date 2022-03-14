using System.CommandLine;
using System.CommandLine.IO;
using System.IO.Abstractions;
using Utilities.Commands;

var utilitiesCli = new UtilsRootCommand(new SystemConsole(), new FileSystem());
await utilitiesCli.InvokeAsync(args);
