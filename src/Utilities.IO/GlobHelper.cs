using Microsoft.Extensions.FileSystemGlobbing;

namespace Utilities.IO;
public static class GlobHelper
{
    public static Matcher CreateGlobMatcher(IEnumerable<string> globs)
    {
        var matcher = new Matcher();

        globs = globs as List<string> ?? globs.ToList();

        var exclude = globs.Where(g => g[0] == '!').ToArray();
        var include = globs.Except(exclude);

        foreach (var glob in exclude) matcher.AddExclude(glob);
        foreach (var glob in include) matcher.AddInclude(glob);

        return matcher;
    }
}
