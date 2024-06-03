using LibGit2Sharp;

namespace Samuel.Infrastructure.Git;

public class RepositoryFactory : IRepositoryFactory
{
    public Repository Create()
    {
        // Add local repository.
        var repositoryPath = Repository.Discover(".");

        if (repositoryPath == null)
        {
            throw new ArgumentException("No repository found.");
        }

        return new Repository(repositoryPath);
    }
}
