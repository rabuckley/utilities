using System.CommandLine;
using System.CommandLine.IO;
using Utilities;
using Xunit;

namespace UtilitiesTests;

public class UtilsCommandTests
{
    private readonly IConsole _console;

    public UtilsCommandTests()
    {
        _console = new TestConsole();
    }

    [Fact] public void ParsesRenameCommandWithGlob()
    {
        // Arrange
        var utils = UtilitiesCli.CreateApp();

        // Act
        var actual = utils.Parse("rename *.md --glob");

        // Assert
        Assert.Equal(3, actual.Tokens.Count);
        Assert.Equal(0, actual.UnmatchedTokens.Count);
    }

    [Fact]
    public void ParsesRenameCommandWithFile()
    {
        var utils = UtilitiesCli.CreateApp();

        var actual = utils.Parse("rename file.md");

        Assert.Equal(2, actual.Tokens.Count);
        Assert.Equal(0, actual.UnmatchedTokens.Count);
    }
}
