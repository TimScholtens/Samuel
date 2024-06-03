using LibGit2Sharp;

namespace Samuel.Infrastructure.Git;

public interface IRepositoryFactory
{
    Repository Create();
}