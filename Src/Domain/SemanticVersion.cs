using System.Text.RegularExpressions;

namespace Samuel.Domain;

public record SemanticVersion(int Major, int Minor, int Patch)
{
    public SemanticVersion() : this(1, 0, 0) { }

    public SemanticVersion Increment(CommitType? type)
    {
        return type switch
        {
            CommitType.Breaking => new SemanticVersion(Major + 1, Minor, Patch),
            CommitType.Feature => new SemanticVersion(Major, Minor + 1, Patch),
            CommitType.Fix => new SemanticVersion(Major, Minor, Patch + 1),
            _ => throw new Exception("New version could not be determined."),
        };
    }

    public static SemanticVersion FromString(string versionString)
    {
        var regex = new Regex("(\\d*).(\\d*).(\\d*)");
        var result = regex.Match(versionString);

        if (!result.Success)
        {
            throw new Exception("Non valid version found.");
        }

        var (major, minor, patch) = (int.Parse(result.Groups[1].Value!), int.Parse(result.Groups[2].Value!), int.Parse(result.Groups[3].Value!));

        return new SemanticVersion(major, minor, patch);
    }

    public override string ToString()
    {
        return $"{Major}.{Minor}.{Patch}";
    }
}
