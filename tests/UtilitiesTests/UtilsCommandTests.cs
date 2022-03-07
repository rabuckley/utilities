using System.CommandLine;
using Utilities;
using Xunit;

namespace UtilitiesTests;

public class UtilsCommandTests
{
    [Theory]
    [InlineData("rename", 1)]
    [InlineData("rename -h", 2)]
    [InlineData("rename file.ext", 2)]
    public void Parse_ShouldHaveNTokens_WhenNTokensArePassed(string command, int expectedTokenCount)
    {
        // Arrange
        var utils = UtilitiesCli.CreateApp();

        // Act
        var actual = utils.Parse(command);

        // Assert
        Assert.Empty(actual.UnmatchedTokens);
        Assert.Equal(expectedTokenCount, actual.Tokens.Count);
    }
}
