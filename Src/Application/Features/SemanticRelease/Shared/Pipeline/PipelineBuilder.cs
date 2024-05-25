using Samuel.Application.Features.SemanticRelease.Shared.Pipeline.Step;

namespace Samuel.Application.Features.SemanticRelease.Shared.Pipeline;

internal class PipelineBuilder
{
    private List<IPipelineStep> _steps;

    internal PipelineBuilder()
    {
        _steps = new List<IPipelineStep>();
    }

    internal PipelineBuilder AddNextStep(IPipelineStep pipelineStep)
    {
        _steps.Add(pipelineStep);
        return this;
    }

    internal Pipeline Build()
    {
        return new Pipeline(_steps);
    }
}
