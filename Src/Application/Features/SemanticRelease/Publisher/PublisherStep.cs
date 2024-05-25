using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Samuel.Application.Features.SemanticRelease.Shared.Git;
using Samuel.Application.Features.SemanticRelease.Shared.Pipeline;
using Samuel.Application.Features.SemanticRelease.Shared.Pipeline.Step;

namespace Samuel.Application.Features.SemanticRelease.Publisher;

public class PublisherStep : IPipelineStep
{
    private readonly IGitService _gitService;
    private readonly ILogger<SemanticReleaser> _logger;
    private readonly PublisherOptions _publisherConfiguration;

    public PublisherStep(IGitService gitService, IOptions<PublisherOptions> publisherConfigurationOptions, ILogger<SemanticReleaser> logger)
    {
        _gitService = gitService;
        _logger = logger;
        _publisherConfiguration = publisherConfigurationOptions.Value;
    }

    public void Execute(PipelineContext context, PipelineCancellationToken cancellationToken)
    {
        if (context.Settings.IsDryRun)
        {
            _logger.LogInformation($"{nameof(PublisherStep)}: Skipping tagging & commit...");
            return;
        }

        // Set and push tag
        _gitService.Tag(context.NewRelease.Tag.Version.ToString());
        _logger.LogInformation($"{nameof(PublisherStep)}: Tagging new version '{context.NewRelease.Tag.Version}'");

        _logger.LogInformation($"{nameof(PublisherStep)}: Push tag");
        _gitService.PushTag(context.NewRelease.Tag.Version.ToString());

        // Commit files and push
        _logger.LogInformation($"{nameof(PublisherStep)}: Staging file '{_publisherConfiguration.PublishFile}'");
        _gitService.StageFile(_publisherConfiguration.PublishFile);

        _logger.LogInformation($"{nameof(PublisherStep)}: Commit '{_publisherConfiguration.PublishFile}'");
        _gitService.CommitAll(_publisherConfiguration.CommitMessage);

        _logger.LogInformation($"{nameof(PublisherStep)}: Push commit");
        _gitService.PushCommit();
    }
}
