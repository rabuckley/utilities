using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using FluentAssertions;
using Utilities.CLI;
using Utilities.Commands;
using Utilities.Commands.Rename;
using Utilities.IO;
using Xunit;

namespace UtilitiesTests;

public class RenameCommandTests
{
    private readonly RenameCommand _renameCommand;
    private readonly RenameCommandHandler _handler;
    private readonly IConsole _console = new TestConsole();
    private readonly MockFileSystem _fileSystem;

    public RenameCommandTests()
    {
        _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>());
        var fileRenamer = new FileRenamer(_fileSystem);
        _handler = new RenameCommandHandler(_console, fileRenamer, _fileSystem);
        _renameCommand = new RenameCommand(_console, _fileSystem, fileRenamer);
    }

    [Theory]
    [InlineData("file.md", 1)]
    [InlineData("*.md", 1)]
    [InlineData("file.md file-2.md", 2)]
    public void Parse_ShouldMatchNTokens_WhenOneNTokensArePassed(string command, int expectedTokens)
    {
        // Act
        var actual = _renameCommand.Parse(command);

        // Assert
        actual.UnmatchedTokens.Should().BeEmpty();
        actual.Tokens.Count.Should().Be(expectedTokens);
    }

    [Fact]
    public async Task Handler_ShouldFail_WhenGivenNonExistentFile()
    {
        var actualResult = await _renameCommand.InvokeAsync("fileDoesNotExist.md");

        actualResult.Should().Be(InvokeResult.Failure);
    }

    [Theory]
    [InlineData("file test.md", "file-test.md")]
    [InlineData("file      test.md", "file-test.md")]
    [InlineData("    file   .   test.md", "file-test.md")]
    [InlineData("file    .....  test.md", "file-test.md")]
    public async Task Handler_ShouldRenameFile_WhenFileExists(string fileName, string expectedName)
    {
        // Arrange
        _fileSystem.File.Create(fileName);
        var files = new FileInfo[] {new(fileName)};

        // Act
        await _handler.ExecuteAsync(files);

        // Assert
        _fileSystem.FileExists(expectedName).Should().BeTrue();
    }
}
