namespace Samuel.Application.Features.SemanticRelease.ApplicationVersionChecker;

public interface INugetService
{
    Version? GetLatestNugetVersion(string packageName);
}
