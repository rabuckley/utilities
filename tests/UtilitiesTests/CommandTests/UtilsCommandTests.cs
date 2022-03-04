using System.CommandLine;
using Utilities;
using Utilities.Commands;
using Xunit;

namespace UtilitiesTests.CommandTests;

public class UtilsCommandTests
{
    [Fact] public void CanCallRenameCommandWithGlob()
    {
        var utils = UtilitiesCli.CreateRootCommand();
        var actual = utils.Parse("rename *.md --glob");
        Assert.Equal(3, actual.Tokens.Count);
    }

    [Fact]
    public void CanCallRenameCommandWithFile()
    {
        var utils = UtilitiesCli.CreateRootCommand();
        var actual = utils.Parse("rename file.md");
        Assert.Equal(2, actual.Tokens.Count);
    }
}
