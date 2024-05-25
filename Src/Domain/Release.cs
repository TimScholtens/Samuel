namespace Samuel.Domain;

public class Release
{
    public AnnotatedTag? Tag { get; set; }
    public required List<Commit> Commits { get; set; }
}
