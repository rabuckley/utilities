using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Commands;
using Xunit;

namespace UtilitiesTests;

public class UtilsCommandTests
{
    [Fact]
    public void CanCallRenameCommand()
    {
        var utils = new UtilsRootCommand(new List<Command> {new RenameCommand()});

        var actual = utils.Parse("rename *.md --glob");

        Assert.Equal(3, actual.Tokens.Count);
    }
}
