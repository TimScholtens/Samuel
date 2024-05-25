namespace Samuel.E2E.Tests.Helpers;

internal static class ChangelogReader
{
    internal static string GetChangeLogContent(string path)
    {
        return File.ReadAllText(path);
    }
}
