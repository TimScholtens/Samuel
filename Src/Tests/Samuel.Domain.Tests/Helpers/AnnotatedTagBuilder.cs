namespace Samuel.Domain.Tests.Helpers;

public class AnnotatedTagBuilder
{
    private SemanticVersion _version;
    private string _commitSha;
    private string _name;

    public AnnotatedTagBuilder()
    {
        _version = new SemanticVersion();
        _name = "tag-1";
        _commitSha = Guid.NewGuid().ToString();
    }

    public AnnotatedTagBuilder WithVersion(string version)
    {
        _version = SemanticVersion.FromString(version);
        return this;
    }

    public AnnotatedTagBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public AnnotatedTag Build()
    {
        return new AnnotatedTag()
        {
            Version = _version,
            CommitSha = _commitSha,
            Name = _name,
            Date = DateOnly.FromDateTime(DateTime.UtcNow)
        };
    }
}
