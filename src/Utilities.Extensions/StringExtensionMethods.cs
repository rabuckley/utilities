namespace Utilities.Extensions;

public static class StringExtensionMethods
{
    public static string ReplaceAll(this string target, string remove, string replace)
    {
        if (!target.Contains(remove)) return target;

        while (target.Contains(remove))
        {
            target = target.Replace(remove, replace);
        }

        return target;
    }

    public static string ReplaceAllDotsWithDashExceptFinal(this string filePath)
    {
        var (body, extension) = filePath.SeparateBodyAndExtensionFromPath();

        body = body.ReplaceAll(".", "-");

        return body + extension;
    }


    public static (string, string) SeparateBodyAndExtensionFromPath(this string path)
    {
        var extensionDotIndex = path.LastIndexOf('.');
        var pathArray = path.ToCharArray();

        List<char> pathBody = new();

        for (var i = 0; i < extensionDotIndex; i++)
        {
            pathBody.Add(pathArray[i]);
        }

        List<char> pathExtension = new();

        for (var i = extensionDotIndex; i < pathArray.Length; i++)
        {
            pathExtension.Add(pathArray[i]);
        }

        var body = new string(pathBody.ToArray());
        var extension = new string(pathExtension.ToArray());

        return (body, extension);
    }
}
