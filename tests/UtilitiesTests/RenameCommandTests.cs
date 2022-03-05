using System;
using System.CommandLine;
using System.CommandLine.IO;
using System.IO;
using System.Linq;
using Utilities.Commands;
using Xunit;

namespace UtilitiesTests;

public class RenameCommandTests
{
    private readonly IConsole _console;

    public RenameCommandTests()
    {
        _console = new TestConsole();
    }

    [Fact]
    public void AllParametersAreMatchedWithGlob()
    {
        var renameCommand = new RenameCommand();

        var actual = renameCommand.Parse("*.md --glob");

        Assert.False(actual.UnmatchedTokens.Any());
        Assert.Equal(2, actual.Tokens.Count);
    }

    [Fact]
    public void AllParametersAreMatchedWithFile()
    {
        var renameCommand = new RenameCommand();

        var actual = renameCommand.Parse("file.md");

        Assert.False(actual.UnmatchedTokens.Any());
        Assert.Equal(1, actual.Tokens.Count);
    }

    [Fact]
    public void RenamesCorrectly()
    {
        // Arrange
        var renameCommand = new RenameCommand();
        EnsureFilesExists("README.md");

        // Act
        renameCommand.Invoke("README.md");

        // Assert
        Assert.True(File.Exists("readme.md"));
    }

    private static void EnsureFilesExists(string fileName)
    {
        if (!File.Exists(fileName)) File.Create(fileName);
        if (!File.Exists(fileName)) throw new Exception("Couldn't find file");
    }
}