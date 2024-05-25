using LibGit2Sharp;
using Samuel.Domain;

namespace Samuel.Infrastructure.Git;

public interface IGitServiceMapper
{
    public Domain.Commit Map(LibGit2Sharp.Commit commit);
    public AnnotatedTag? Map(Tag tag);
}