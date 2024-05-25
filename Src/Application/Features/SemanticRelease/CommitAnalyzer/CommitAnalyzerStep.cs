using Microsoft.Extensions.Logging;
using Samuel.Application.Features.SemanticRelease.Shared.Pipeline;
using Samuel.Application.Features.SemanticRelease.Shared.Pipeline.Step;
using Samuel.Domain;

namespace Samuel.Application.Features.SemanticRelease.CommitAnalyzer;

public class CommitAnalyzerStep : IPipelineStep
{
    private readonly ILogger<SemanticReleaser> _logger;

    public CommitAnalyzerStep(ILogger<SemanticReleaser> logger)
    {
        _logger = logger;
    }

    public void Execute(PipelineContext context, PipelineCancellationToken cancellationToken)
    {
        if (context.NewRelease.Commits is null)
        {
            cancellationToken.IsCancelled = true;
            _logger.LogInformation($"{nameof(CommitAnalyzerStep)}: No new commit(s) found...");
            return;
        }

        if (!context.NewRelease.Commits.Any(c => c.Type == CommitType.Feature || c.Type == CommitType.Breaking))
        {
            cancellationToken.IsCancelled = true;
            _logger.LogInformation($"{nameof(CommitAnalyzerStep)}: No new features/breaking changes found in new commit(s)...");
            return;
        }
    }


}
