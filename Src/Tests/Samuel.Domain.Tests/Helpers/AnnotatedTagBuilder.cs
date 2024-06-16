namespace Samuel.Domain.Tests.Helpers;

public class AnnotatedTagBuilder
{
    private SemanticVersion _version;
    private string _name;
    private Commit _commit;

    public AnnotatedTagBuilder()
    {
        _version = new SemanticVersion();
        _name = "tag-1";
        _commit = new CommitBuilder().Build();
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

    public AnnotatedTagBuilder WithCommit(Commit commit)
    {
        _commit = commit;
        return this;
    }

    public AnnotatedTag Build()
    {
        return new AnnotatedTag()
        {
            Version = _version,
            Name = _name,
            Commit = _commit
        };
    }
}
