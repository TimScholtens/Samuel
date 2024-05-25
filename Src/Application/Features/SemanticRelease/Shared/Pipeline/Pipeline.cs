using Samuel.Application.Features.SemanticRelease.Shared.Pipeline.Step;

namespace Samuel.Application.Features.SemanticRelease.Shared.Pipeline;

public class Pipeline
{
    private readonly List<IPipelineStep> _pipelineSteps;

    public Pipeline(List<IPipelineStep> steps)
    {
        _pipelineSteps = steps;
    }

    public void Run(SemanticReleaserSettings settings)
    {
        var cancellationToken = new PipelineCancellationToken();
        var context = new PipelineContext()
        {
            Settings = settings
        };

        foreach (var step in _pipelineSteps)
        {
            step.Execute(context, cancellationToken);

            if (cancellationToken.IsCancelled)
            {
                break;
            }
        }
    }

    internal static PipelineBuilder CreateBuilder()
    {
        return new PipelineBuilder();
    }
}
