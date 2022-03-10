using FluentAssertions;
using System.CommandLine;
using System.CommandLine.IO;
using System.IO.Abstractions.TestingHelpers;
using Utilities.CLI;
using Utilities.Commands;
using Xunit;

namespace UtilitiesTests;

public class UtilsCommandTests
{
    private readonly UtilsRootCommand _sut;

    public UtilsCommandTests()
    {
        _sut = new UtilsRootCommand(new TestConsole(), new MockFileSystem());
    }

    [Theory]
    [InlineData("rename -h", 2)]
    [InlineData("rename file.ext", 2)]
    public void Parse_ShouldHaveNTokens_WhenNTokensArePassed(string command, int expectedTokenCount)
    {
        // Act
        var actual = _sut.Parse(command);

        // Assert
        actual.UnmatchedTokens.Should().BeEmpty();
        actual.Tokens.Count.Should().Be(expectedTokenCount);
    }

    [Theory]
    [InlineData("rename -h")]
    [InlineData("extract -h")]
    public void Invoke_ShouldReturnSuccess_WhenGivenAValidCommand(string command)
    {
        // Act
        var actual = _sut.Invoke(command);

        // Assert
        actual.Should().Be(InvokeResult.Success);
    }

    [Theory]
    [InlineData("rename")]
    [InlineData("extract")]
    public void Invoke_ShouldReturnFail_WhenGivenAnInvalidCommand(string command)
    {
        // Act
        var actual = _sut.Invoke(command);

        // Assert
        actual.Should().Be(InvokeResult.Failure);
    }
}
