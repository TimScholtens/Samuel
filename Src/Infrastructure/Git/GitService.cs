using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Samuel.Application.Features.SemanticRelease.Shared.Git;
using Samuel.Domain;

namespace Samuel.Infrastructure.Git;

public class GitService : IGitService
{
    private readonly GitOptions _configuration;
    private readonly IGitServiceMapper _mapper;
    private readonly ILogger<GitService> _logger;
    private readonly Repository _repository;
    private readonly CredentialsHandler _credentialsHandler;

    public GitService(IOptions<GitOptions> configurationOptions, IGitServiceMapper mapper, Repository? repository, ILogger<GitService> logger)
    {
        _configuration = configurationOptions.Value;
        _mapper = mapper;
        _logger = logger;
        _repository = repository!;
        _credentialsHandler = new CredentialsHandler((url, usernameFromUrl, types) =>
            new UsernamePasswordCredentials()
            {
                Username = _configuration.Username,
                Password = _configuration.Password ?? _configuration.Credentials
            });
        if (repository == null)
        {
            throw new ArgumentException("No repository found.");
        }
    }

    public Domain.Commit? GetCommit(string sha)
    {
        var commit = _repository.Commits.FirstOrDefault(c => c.Sha == sha);

        if (commit != null)
        {
            return _mapper.Map(commit);
        }

        return null;
    }

    public List<Domain.Commit> GetCommits()
    {
        return _repository.Commits.Select(c => _mapper.Map(c)).ToList();
    }

    /// <summary>
    /// Returns commits that happened after given commit.
    /// </summary>
    /// <param name="commitSha">SHA of commit of interest</param>
    /// <returns>Returns all commits that happened after given commit, excluding given commit.</returns>
    public List<Domain.Commit> GetCommitsAfter(string commitSha)
    {
        var commitIndex = _repository.Commits.ToList().FindIndex(c => c.Sha == commitSha);

        if (commitIndex == -1)
        {
            throw new Exception("Given commit SHA doesn't exist.");
        }

        var newCommits = _repository.Commits.ToList()[..commitIndex];

        return newCommits.Select(c => _mapper.Map(c)).ToList();
    }

    /// <summary>
    /// Returns commits that happened before given commit.
    /// </summary>
    /// <param name="commitSha">SHA of commit of interest</param>
    /// <returns>Returns all commits that happened before given commit, excluding given commit.</returns>
    public List<Domain.Commit> GetCommitsBefore(string commitSha)
    {
        var commitIndex = _repository.Commits.ToList().FindIndex(c => c.Sha == commitSha);
        var commitsBefore = _repository.Commits.ToList()[(commitIndex + 1)..];

        return commitsBefore.Select(c => _mapper.Map(c)).ToList();
    }

    /// <summary>
    /// Returns commits that happened between given commits.
    /// </summary>
    /// <param name="commitSha">SHA's of commits of interest</param>
    /// <returns>Returns all commits that happened between given commits, excluding given commits.</returns>
    public List<Domain.Commit> GetCommitsBetween(string fromCommitSha, string toCommitSha)
    {
        var commits = _repository.Commits.ToList();
        var fromCommitIndex = _repository.Commits.ToList().FindIndex(c => c.Sha == fromCommitSha);
        var toCommitIndex = _repository.Commits.ToList().FindIndex(c => c.Sha == toCommitSha);

        var commitsBetweenIndexes = _repository.Commits.ToList()[toCommitIndex..(fromCommitIndex - 1)];

        return commitsBetweenIndexes.Select(c => _mapper.Map(c)).ToList();
    }

    public Domain.Release? GetLatestRelease()
    {
        var latestTags = _repository.Tags
            .Select(t => _mapper.Map(t))
            .OrderByDescending(t => t.Version.Major)
            .OrderByDescending(t => t.Version.Minor)
            .OrderByDescending(t => t.Version.Patch)
            .Take(2)
            .ToArray();

        // If no tags.
        if (latestTags?.Any() != true)
        {
            return null;
        }

        var latestTag = latestTags[0];

        // If only 1 tag.
        if (latestTags.Length == 1)
        {
            var commitsBeforeLatestTag = GetCommitsBefore(latestTag.CommitSha);

            return new Release()
            {
                Tag = latestTag,
                Commits = commitsBeforeLatestTag,
            };
        }

        // If more than 1 tag.
        var secondLatestTag = latestTags[1];
        var commitsBetweenLatestTags = GetCommitsBetween(secondLatestTag.CommitSha, latestTag.CommitSha);

        return new Release()
        {
            Tag = latestTag,
            Commits = commitsBetweenLatestTags,
        };
    }

    public List<Release> GetAllReleases()
    {
        var releases = new List<Release>();
        var tags = _repository.Tags
            .Select(t => _mapper.Map(t))
            .OrderBy(t => t.Version.Major)
            .ThenBy(t => t.Version.Minor)
            .ThenBy(t => t.Version.Patch)
            .ToList();

        for (int index = 0; index < tags.Count; index++)
        {
            var currentTag = tags[index];
            var currentCommit = GetCommit(currentTag.CommitSha);

            // If no previous tag.
            if (index == 0)
            {
                releases.Add(new Release()
                {
                    Commits = [currentCommit, ..GetCommitsBefore(currentTag.CommitSha)],
                    Tag = currentTag
                });
                continue;
            }

            var previousTag = tags[index - 1];

            _logger.LogDebug($"{nameof(GitService)}: Retrieving commits between tags {previousTag.Name}({previousTag.CommitSha}) to {currentTag.Name}({currentTag.CommitSha})...");
            List<Domain.Commit> commitsBetweenTags = [currentCommit, .. GetCommitsBetween(previousTag.CommitSha, currentTag.CommitSha)];

            _logger.LogDebug($"{nameof(GitService)}: Found {commitsBetweenTags.Count} commits.");

            releases.Add(new Release
            {
                Commits = commitsBetweenTags,
                Tag = currentTag
            });
        }

        return releases;
    }

    public void PushCommit()
    {
        var pushOptions = new PushOptions() { CredentialsProvider = _credentialsHandler };
        _logger.LogDebug($"{nameof(GitService)}: Starting commit.");
        var remote = _repository.Network.Remotes["origin"];
        var currentBranch = _repository.Head.TrackedBranch;
        _logger.LogDebug($"{nameof(GitService)}: Current branch {currentBranch.UpstreamBranchCanonicalName}");
        _repository.Network.Push(remote, currentBranch.UpstreamBranchCanonicalName, pushOptions);
    }

    public void PushTag(string tagName)
    {
        var pushOptions = new PushOptions()
        {
            CredentialsProvider = _credentialsHandler
        };
        var remote = _repository.Network.Remotes["origin"];
        _repository.Network.Push(remote, $"refs/tags/{tagName}", pushOptions);
    }

    public void CommitAll(string commitMessage)
    {
        _repository.Commit(commitMessage, new Signature(_configuration.CommiterName, _configuration.CommiterEmail, DateTimeOffset.UtcNow), new Signature(_configuration.CommiterName, _configuration.CommiterEmail, DateTimeOffset.UtcNow), new CommitOptions() { });
    }

    public void StageFile(string filePath)
    {
        // Get relative path from .git folder to application working directory.
        var absoluteFilePath = Path.Combine(Environment.CurrentDirectory, filePath);
        var relativeFilePathToRoot = Path.GetRelativePath(_repository.Info.WorkingDirectory, absoluteFilePath);
        _repository.Index.Add(relativeFilePathToRoot);
        _repository.Index.Write();
    }

    public void Tag(string newVersionName)
    {
        _repository.ApplyTag(newVersionName);
    }
}
