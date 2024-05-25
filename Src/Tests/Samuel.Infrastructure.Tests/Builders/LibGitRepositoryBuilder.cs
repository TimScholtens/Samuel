using LibGit2Sharp;
using System.IO;

namespace Samuel.Infrastructure.Tests.Helpers;

public class LibGitRepositoryBuilder
{
    private readonly Repository _repository;
    private string _repoPath;

    public LibGitRepositoryBuilder(string? path = null)
    {
        _repoPath = path ?? Path.Combine(Path.GetTempPath(), $"samuel-{Guid.NewGuid()}");
        Directory.CreateDirectory(_repoPath);
        Repository.Init(_repoPath);
        _repository = new Repository(_repoPath);
    }

    public LibGitRepositoryBuilder WithCommit(string message)
    {
        // Create & stage file.
        var filePath = Path.Combine(_repoPath, $"{Guid.NewGuid()}.txt");
        File.Create(filePath).Dispose();

        var relativeFilePathToRoot = Path.GetRelativePath(_repository.Info.WorkingDirectory, filePath);
        _repository.Index.Add(relativeFilePathToRoot);
        _repository.Index.Write();
        _repository.Commit(message, new Signature("bot", "bot@noreply.com", DateTimeOffset.UtcNow), new Signature("bot", "bot@noreply.com", DateTimeOffset.UtcNow));
        return this;
    }

    public LibGitRepositoryBuilder WithManyCommits(int numberOfCommits, string commitMessage)
    {
        for (int i = 0; i < numberOfCommits; i++)
        {
            // Create & stage file.
            var filePath = Path.Combine(_repoPath, $"{Guid.NewGuid()}.txt");
            File.Create(filePath).Dispose();

            var relativeFilePathToRoot = Path.GetRelativePath(_repository.Info.WorkingDirectory, filePath);
            _repository.Index.Add(relativeFilePathToRoot);
            _repository.Index.Write();
            _repository.Commit($"{commitMessage}-{i}", new Signature("bot", "bot@noreply.com", DateTimeOffset.UtcNow), new Signature("bot", "bot@noreply.com", DateTimeOffset.UtcNow));
        }

        return this;
    }

    public LibGitRepositoryBuilder WithTag(string tagName)
    {
        _repository.ApplyTag(tagName);
        return this;
    }

    public Repository Build()
    {
        return _repository;
    }
}
