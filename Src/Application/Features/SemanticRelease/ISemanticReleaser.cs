namespace Samuel.Application.Features.SemanticRelease;

public interface ISemanticReleaser
{
    int Run(SemanticReleaserSettings settings);
}