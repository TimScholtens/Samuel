namespace Samuel.Application.Features.SemanticRelease.Changelog;

public interface IChangelogWriter
{
    public void WriteTitle(string title);

    public void WriteLine();

    public void WriteSubTitle(string title);

    public void WriteHeader(string header);

    public void WriteChange(string text);

    public void Flush();
}
