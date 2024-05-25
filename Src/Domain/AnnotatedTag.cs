namespace Samuel.Domain;

public class AnnotatedTag
{
    public required SemanticVersion Version { get; set; }
    public string CommitSha { get; set; }
    public string Name { get; set; }
}
