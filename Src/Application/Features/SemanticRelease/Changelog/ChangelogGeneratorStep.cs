using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Samuel.Application.Features.SemanticRelease.Shared.Git;
using Samuel.Application.Features.SemanticRelease.Shared.Pipeline;
using Samuel.Application.Features.SemanticRelease.Shared.Pipeline.Step;
using Samuel.Domain;

namespace Samuel.Application.Features.SemanticRelease.Changelog;

public class ChangelogGeneratorStep : IPipelineStep
{
    private readonly ChangelogGeneratorOptions _config;
    private readonly IGitService _gitService;
    private readonly ILogger<SemanticReleaser> _logger;
    private readonly IChangelogWriter _writer;

    public ChangelogGeneratorStep(IOptions<ChangelogGeneratorOptions> configurationOptions, IGitService gitService, ILogger<SemanticReleaser> logger, IChangelogWriter writer)
    {
        _config = configurationOptions.Value;
        _gitService = gitService;
        _logger = logger;
        _writer = writer;
    }

    public void Execute(PipelineContext context, PipelineCancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(ChangelogGeneratorStep)}: Generating changelog...");
        var previousReleases = _gitService.GetAllReleases();
        var allReleases = previousReleases.Append(context.NewRelease);
        var reversedOrderAllReleases = allReleases.Reverse();

        _writer.WriteTitle(_config.Title);

        foreach (var release in reversedOrderAllReleases.ToList())
        {
            _writer.WriteHeader($"{release.Tag.Version}");

            // Write features.
            _writer.WriteSubTitle(_config.FeatureSectionTitle);
            foreach (var commit in release.Commits!.Where(c => c.Type == CommitType.Feature || c.Type == CommitType.Breaking))
            {
                var commitMessage = FormatCommitMessage(commit, _config.IssueUrlFormat);
                _writer.WriteChange(commitMessage);
            }

            if (release.Commits.Any(c => c.Type == CommitType.Fix))
            {
                _writer.WriteLine();

                // Write fixes.
                _writer.WriteSubTitle(_config.FixSectionTitle);
                foreach (var commit in release.Commits!.Where(c => c.Type == CommitType.Fix))
                {
                    var commitMessage = FormatCommitMessage(commit, _config.IssueUrlFormat);
                    _writer.WriteChange(commitMessage);
                }
            }

            _writer.WriteLine();
        }

        // Write all changes to file.
        _writer.Flush();

        _logger.LogInformation($"{nameof(ChangelogGeneratorStep)}: Changelog generated.");
    }

    private string FormatCommitMessage(Commit commit, string? issueUrlFormat)
    {
        if (issueUrlFormat is not null && commit.RelatedWorkItemsIds?.Length > 0)
        {
            var issueIdsWithUrl = commit.RelatedWorkItemsIds.Select(i => $"[{i}]({_config.IssueUrlFormat}/{i})");
            return $"{commit.Title}, closes issue(s): {string.Join(',', issueIdsWithUrl)}.";
        }

        return commit.Title;
    }
}

