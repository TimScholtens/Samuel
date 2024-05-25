using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Samuel.Application.Features.SemanticRelease.ApplicationVersionChecker;
using Samuel.Application.Features.SemanticRelease.Changelog;
using Samuel.Application.Features.SemanticRelease.CommitAnalyzer;
using Samuel.Application.Features.SemanticRelease.Publisher;
using Samuel.Application.Features.SemanticRelease.ReleaseAnalyzer;
using Samuel.Application.Features.SemanticRelease.SemanticVersionUpdater;
using Samuel.Application.Features.SemanticRelease.Shared.Git;
using Samuel.Application.Features.SemanticRelease.Shared.Pipeline;

namespace Samuel.Application.Features.SemanticRelease;

public class SemanticReleaser : ISemanticReleaser
{
    private readonly ILogger<SemanticReleaser> _logger;
    private readonly Pipeline _semanticReleasePipeline;

    public SemanticReleaser(IGitService gitService, INugetService nugetService, IChangelogWriter writer, IOptions<ChangelogGeneratorOptions> changelogGeneratorOptions, IOptions<PublisherOptions> publisherOptions, ILogger<SemanticReleaser> logger)
    {
        _logger = logger;
        _semanticReleasePipeline = Pipeline.CreateBuilder()
            .AddNextStep(new ApplicationVersionCheckerStep(nugetService, logger))
            .AddNextStep(new ReleaseAnalyzerStep(gitService, logger))
            .AddNextStep(new CommitAnalyzerStep(logger))
            .AddNextStep(new SemanticVersionUpdaterStep(logger))
            .AddNextStep(new ChangelogGeneratorStep(changelogGeneratorOptions, gitService, logger, writer))
            .AddNextStep(new PublisherStep(gitService, publisherOptions, logger))
            .Build();
    }

    public int Run(SemanticReleaserSettings settings)
    {
        try
        {
            _semanticReleasePipeline.Run(settings);
            return 0;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return -99;
        }
    }
}
