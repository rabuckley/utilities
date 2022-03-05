using System.CommandLine;
using Utilities;

var utilitiesCli = UtilitiesCli.CreateApp();
await utilitiesCli.InvokeAsync(args);
