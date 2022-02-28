using System.CommandLine;
using Utilities.Commands;

var renameCommand = new RenameCommand();


var commands = new Command[] { renameCommand.Command };

var utils = new UtilsRootCommand(commands, null, null);

await utils.RootCommand.InvokeAsync(args);