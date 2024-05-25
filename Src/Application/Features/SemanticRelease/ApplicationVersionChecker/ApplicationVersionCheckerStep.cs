using Microsoft.Extensions.Logging;
using Samuel.Application.Features.SemanticRelease.Shared.Pipeline;
using Samuel.Application.Features.SemanticRelease.Shared.Pipeline.Step;

namespace Samuel.Application.Features.SemanticRelease.ApplicationVersionChecker;

public class ApplicationVersionCheckerStep : IPipelineStep
{
    private readonly INugetService _nugetService;
    private readonly ILogger<SemanticReleaser> _logger;

    public ApplicationVersionCheckerStep(INugetService nugetService, ILogger<SemanticReleaser> logger)
    {
        _nugetService = nugetService;
        _logger = logger;
    }

    public void Execute(PipelineContext context, PipelineCancellationToken cancellationToken)
    {
        var nugetVersion = _nugetService.GetLatestNugetVersion("samuel");

        if (nugetVersion == null)
        {
            return;
        }
        var assemblyVersion = (typeof(ApplicationVersionCheckerStep).Assembly.GetName().Version!);

        if (assemblyVersion < nugetVersion)
        {
            _logger.LogInformation("Use 'dotnet tool update samuel' to update to the latest version.");
        }
    }
}
