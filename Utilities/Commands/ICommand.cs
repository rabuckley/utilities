using System.CommandLine;

namespace Utilities.Commands
{
    public interface ICommand
    {
        public Command Command { get; set; }
    }
}