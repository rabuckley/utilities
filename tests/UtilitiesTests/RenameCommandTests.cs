using System.CommandLine;
using System.Linq;
using Utilities.Commands;
using Xunit;

namespace UtilitiesTests;
public class RenameCommandTests
{
    [Fact]
    public void AllParametersAreMatchedWithGlob()
    {
        var renameCommand = new RenameCommand();

        const string glob = "*.md --glob";

        var actual = renameCommand.Parse(glob);

        Assert.False(actual.UnmatchedTokens.Any());
        Assert.Equal(2, actual.Tokens.Count);
    }

    [Fact]
    public void AllParametersAreMatchedWithFile()
    {
        var renameCommand = new RenameCommand();

        const string glob = "file.md";

        var actual = renameCommand.Parse(glob);

        Assert.False(actual.UnmatchedTokens.Any());
        Assert.Equal(1, actual.Tokens.Count);
    }
}