namespace Utilities.Commands;

static class StringExtensionMethods
{
    public static string RemoveAll(this string target, string remove, string replace)
    {
        if (!target.Contains(remove)) return target;

        while (target.Contains(remove))
            target = target.Replace(remove, replace);

        return target;
    }
}