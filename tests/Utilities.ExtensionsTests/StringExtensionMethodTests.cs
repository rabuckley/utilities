using Utilities.Extensions;
using Xunit;

namespace Utilities.ExtensionsTests;

public class UnitTest1
{
    [Theory]
    [InlineData("readme.md", "readme", ".md")]
    [InlineData("slightly.harder.file.txt", "slightly.harder.file", ".txt")]
    public void SeparatesBodyAndExtensionFromPath(string path, string expectedBody, string expectedExtension)
    {
        var (actualBody, actualExtension) = path.SeparateBodyAndExtensionFromPath();

        Assert.Equal(expectedBody, actualBody);
        Assert.Equal(expectedExtension, actualExtension);

    }

    [Theory]
    [InlineData("README.md", "README.md")]
    [InlineData("test.file.md", "test-file.md")]
    [InlineData("....leading.dots....md", "----leading-dots---.md")]
    [InlineData(".", ".")]
    public void Test1(string path, string expected)
    {
        var actual = path.ReplaceAllDotsWithDashExceptFinal();
        Assert.Equal(expected, actual);
    }


}
