using Samuel.Domain;

namespace Samuel.Application.Features.SemanticRelease.Shared.Pipeline;

public class PipelineContext
{
    public Release? LatestRelease { get; set; }
    public List<Commit>? NewCommits { get; set; }
    public Release? NewRelease { get; set; }
    public SemanticReleaserSettings Settings { get; set; }
}

TODO add diagram.