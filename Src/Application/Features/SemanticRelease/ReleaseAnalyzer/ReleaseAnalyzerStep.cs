using Microsoft.Extensions.Logging;
using Samuel.Application.Features.SemanticRelease.Shared.Git;
using Samuel.Application.Features.SemanticRelease.Shared.Pipeline;
using Samuel.Application.Features.SemanticRelease.Shared.Pipeline.Step;

namespace Samuel.Application.Features.SemanticRelease.ReleaseAnalyzer;

public class ReleaseAnalyzerStep : IPipelineStep
{
    private readonly IGitService _gitService;
    private readonly ILogger<SemanticReleaser> _logger;

    public ReleaseAnalyzerStep(IGitService gitService, ILogger<SemanticReleaser> logger)
    {
        _gitService = gitService;
        _logger = logger;
    }

    public void Execute(PipelineContext context, PipelineCancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(ReleaseAnalyzerStep)}: Get latest release...");

        var latestRelease = _gitService.GetLatestRelease();

        if (latestRelease == null) { _logger.LogInformation($"{nameof(ReleaseAnalyzerStep)}: Found no previous release..."); }
        if (latestRelease != null) { _logger.LogInformation($"{nameof(ReleaseAnalyzerStep)}: Latest release found '{latestRelease.Tag.Name}'"); }

        // Get new commits.
        var newCommits = latestRelease is null ? _gitService.GetCommits() : _gitService.GetCommitsAfter(latestRelease.Tag.Commit.Sha);
        _logger.LogInformation($"{nameof(ReleaseAnalyzerStep)}: Found {newCommits.Count} new commit(s)...");

        // Set new commits & new release.
        context.NewRelease = new Domain.Release()
        {
            Commits = newCommits
        };
        context.LatestRelease = latestRelease;
    }
}
