namespace Samuel.Domain;

public class AnnotatedTag
{
    public required SemanticVersion Version { get; set; }
    public required Commit Commit { get; set; }
    public string Name { get; set; }
}
