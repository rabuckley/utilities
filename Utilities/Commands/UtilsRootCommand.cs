using System.CommandLine;

namespace Utilities.Commands
{
	public class UtilsRootCommand
	{
		public UtilsRootCommand(IEnumerable<Command>? commands, IEnumerable<System.CommandLine.Option>? options, IEnumerable<Argument>? arguments)
		{
            RootCommand = new RootCommand
            {
                Name = "utils",
                Description = "Custom command line utilities."
            };

            if (options is not null)
			{
				foreach (var option in options)
					RootCommand.AddOption(option);
			}

			if (commands is not null)
			{
				foreach (var command in commands)
					RootCommand.AddCommand(command);
			}

			if (arguments is not null)
			{
				foreach (var argument in arguments)
					RootCommand.AddArgument(argument);
			}
		}
        public RootCommand RootCommand { get; set; }
    }
}

