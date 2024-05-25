namespace Samuel.Application.Features.SemanticRelease.Publisher;

public class PublisherOptions
{
    public const string SectionName = "Publisher";
    public required string PublishFile { get; set; }
    public required string CommitMessage { get; set; }
}
