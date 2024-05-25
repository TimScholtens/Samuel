using Microsoft.Extensions.Options;
using System.Text;

namespace Samuel.Application.Features.SemanticRelease.Changelog;

public class MarkdownWriter : IChangelogWriter
{
    private readonly ChangelogGeneratorOptions _configuration;
    private readonly string _filePath;
    private readonly StringBuilder _textBuffer;

    public MarkdownWriter(IOptions<ChangelogGeneratorOptions> options)
    {
        _configuration = options.Value;
        _filePath = Path.Combine(Environment.CurrentDirectory, _configuration.OutputFilePath);
        _textBuffer = new StringBuilder();
    }

    public void WriteChange(string text)
    {
        _textBuffer.Append($"- {text}");
    }

    public void WriteHeader(string text)
    {
        _textBuffer.AppendLine($"## {text}");
    }

    public void WriteTitle(string title)
    {
        _textBuffer.AppendLine($"# {title}");
    }

    public void WriteSubTitle(string subtitle)
    {
        _textBuffer.AppendLine($"*{subtitle}*");
    }

    public void WriteLine()
    {
        _textBuffer.AppendLine(string.Empty);
    }

    public void Flush()
    {
        File.WriteAllText(_filePath, string.Join(Environment.NewLine, _textBuffer.ToString()));
    }
}
