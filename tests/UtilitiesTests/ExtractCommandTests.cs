using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using FluentAssertions;
using Utilities.Commands;
using Utilities.Commands.Extract;
using Xunit;

namespace UtilitiesTests;

public class ExtractCommandTests
{
    private readonly IConsole _console;

    public ExtractCommandTests()
    {
        _console = new TestConsole();
    }

    [Fact]
    public void Handler_ShouldExtractAllFiles_WhenGivenDirectoryWithChildFiles()
    {
        // Arrange
        var fileSystem = new MockFileSystem();
        fileSystem.AddDirectory("/sub1");
        fileSystem.AddDirectory("/sub2");
        fileSystem.AddFile("/sub1/file1.txt", new MockFileData(""));
        fileSystem.AddFile("/sub1/file2.txt", new MockFileData(""));
        fileSystem.AddFile("/sub2/file3.txt", new MockFileData(""));

        var sut = new ExtractCommand(_console, fileSystem);

        // Act
        sut.Handler(new DirectoryInfo("/"));

        // Assert
        fileSystem.Directory.Exists("/sub1").Should().BeFalse();
        fileSystem.Directory.Exists("/sub2").Should().BeFalse();
        fileSystem.FileExists("file1.txt").Should().BeTrue();
        fileSystem.FileExists("file2.txt").Should().BeTrue();
        fileSystem.FileExists("file3.txt").Should().BeTrue();
    }

    [Fact]
    public void Handler_ShouldExtractAllFiles_WhenGivenFilesInNestedSubdirectories()
    {
        var fileNames = new List<string>();

        for (int i = 0; i < 5; i++)
        {
            fileNames.Add($"file-{i}.txt");
        }

        // Arrange
        var fileSystem = new MockFileSystem();
        fileSystem.AddDirectory("/sub1");
        fileSystem.AddDirectory("/sub1/sub2");
        fileSystem.AddDirectory("/sub1/sub2/sub3");
        fileSystem.AddFile($"/sub1/{fileNames[0]}", new MockFileData(""));
        fileSystem.AddFile($"/sub1/{fileNames[1]}", new MockFileData(""));
        fileSystem.AddFile($"/sub1/sub2/{fileNames[2]}", new MockFileData(""));
        fileSystem.AddFile($"/sub1/sub2/{fileNames[3]}", new MockFileData(""));
        fileSystem.AddFile($"/sub1/sub2/sub3/{fileNames[4]}", new MockFileData(""));


        var sut = new ExtractCommandHandler(fileSystem, _console);

        // Act
        sut.Execute(new DirectoryInfo("/"));

        // Assert
        foreach (var file in fileNames)
        {
            fileSystem.FileExists(file).Should().BeTrue();
        }
    }
}
