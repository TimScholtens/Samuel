using Microsoft.Extensions.Options;
using Samuel.Domain;
using System.Text.RegularExpressions;

namespace Samuel.Infrastructure.Git;

public class GitServiceMapper : IGitServiceMapper
{
    private readonly GitOptions _configuration;

    public GitServiceMapper(IOptions<GitOptions> options)
    {
        _configuration = options.Value;
    }

    public Commit Map(LibGit2Sharp.Commit commit)
    {
        // Check for commit work item resolution & description.
        var commitLines = commit.Message.Split('\n').Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
        var title = commitLines.ElementAtOrDefault(0)?.Trim();
        var description = commitLines.ElementAtOrDefault(1)?.Trim();
        var workItems = commitLines.ElementAtOrDefault(2)?.Trim();

        var workItemsIds = RetrieveWorkItems(workItems);
        var commitType = ClassifyCommit(commit.Message);

        return new Commit()
        {
            Id = commit.Id.ToString(),
            Sha = commit.Sha,
            CreatedAt = commit.Author.When.UtcDateTime,
            RawContent = commit.Message,
            Title = title,
            Description = description,
            RelatedWorkItemsIds = workItemsIds,
            Type = commitType
        };
    }

    internal string[] RetrieveWorkItems(string? workItems)
    {
        if (workItems == null) return [];

        var retrieveWorkItemIdsRegex = new Regex("#(\\d+)");
        var workItemsIds = retrieveWorkItemIdsRegex.Matches(workItems);

        if (workItemsIds == null) return [];

        return workItemsIds
            .Select(c => c.Value.Trim())
            .Select(c => c.Replace("#", ""))
            .ToArray();
    }

    internal CommitType ClassifyCommit(string commitMessage)
    {
        var regex = new Regex(_configuration.CommitMessageParseRegex);
        var result = regex.Match(commitMessage);
        var commitType = CommitType.None;

        if (result.Success)
        {
            var (id, type, message) = (result.Groups[1].Value!, result.Groups[2].Value!, result.Groups[3].Value!);
            Enum.TryParse(type, true, out commitType);
        }

        return commitType;
    }

    public AnnotatedTag? Map(LibGit2Sharp.Tag tag)
    {
        return tag is null ? null : new AnnotatedTag()
        {
            Version = SemanticVersion.FromString(tag.FriendlyName),
            CommitSha = tag.Target.Sha,
            Name = tag.FriendlyName,
        };
    }
}
