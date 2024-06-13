using Microsoft.Extensions.Logging;
using Samuel.Application.Features.SemanticRelease.Shared.Pipeline;
using Samuel.Application.Features.SemanticRelease.Shared.Pipeline.Step;

namespace Samuel.Application.Features.SemanticRelease.SemanticVersionUpdater;

public class SemanticVersionUpdaterStep : IPipelineStep
{
    private readonly ILogger<SemanticReleaser> _logger;

    public SemanticVersionUpdaterStep(ILogger<SemanticReleaser> logger)
    {
        _logger = logger;
    }

    public void Execute(PipelineContext context, PipelineCancellationToken cancellationToken)
    {
        // Determine new version.
        var biggestChange = context.NewRelease.Commits.MaxBy(c => c.Type)!;
        _logger.LogInformation($"{nameof(SemanticVersionUpdaterStep)}: Biggest change found '{biggestChange.Title}' ");

        var newVersion = new Domain.SemanticVersion();
        if (context.LatestRelease is not null)
        {
            newVersion = context.LatestRelease.Tag.Version.Increment(biggestChange.Type);
        }

        // Set new version.
        context.NewRelease.Tag = new Domain.AnnotatedTag() { Version = newVersion, Commit = biggestChange };
    }
}
