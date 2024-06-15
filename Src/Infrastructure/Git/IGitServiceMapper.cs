namespace Samuel.Infrastructure.Git;

public interface IGitServiceMapper
{
    public Domain.Commit Map(LibGit2Sharp.Commit commit);
    public Domain.AnnotatedTag? Map(LibGit2Sharp.Tag tag, Domain.Commit commit);
}