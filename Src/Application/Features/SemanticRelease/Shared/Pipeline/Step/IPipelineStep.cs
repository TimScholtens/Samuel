namespace Samuel.Application.Features.SemanticRelease.Shared.Pipeline.Step;

public interface IPipelineStep
{
    public void Execute(PipelineContext context, PipelineCancellationToken cancellationToken);

}
