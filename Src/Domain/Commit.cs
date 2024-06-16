namespace Samuel.Domain;

public record Commit
{
    public required string Id { get; init; }
    public required string Sha { get; init; }
    public required string RawContent { get; init; }
    public required CommitType Type { get; init; }
    public required string Description { get; init; }
    public required string Title { get; init; }
    public required DateOnly CreatedAt { get; init; }
    public required string[]? RelatedWorkItemsIds { get; init; }
}
