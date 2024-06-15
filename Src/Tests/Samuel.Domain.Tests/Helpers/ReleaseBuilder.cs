namespace Samuel.Domain.Tests.Helpers;

public class ReleaseBuilder
{
    private List<Commit> _commits;
    private AnnotatedTag _tag;

    public ReleaseBuilder()
    {
        var tagCommit = new CommitBuilder()
            .WithMessage("TagCommit")
            .Build();

        _commits = new List<Commit>() { tagCommit };
        _tag = new AnnotatedTag()
        {
            Version = new SemanticVersion(),
            Commit = tagCommit
        };
    }

    public ReleaseBuilder WithCommit(Commit commit)
    {
        _commits.Add(commit);
        return this;
    }

    public ReleaseBuilder WithTag(AnnotatedTag tag)
    {
        _tag = tag;
        return this;
    }

    public Release Build()
    {
        return new Release()
        {
            Tag = _tag,
            Commits = _commits
        };
    }

}
