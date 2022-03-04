using System.CommandLine;
using Utilities;

var utilitiesCli = UtilitiesCli.CreateRootCommand();
await utilitiesCli.InvokeAsync(args);