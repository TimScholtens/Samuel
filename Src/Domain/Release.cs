namespace Samuel.Domain;

public class Release
{
    public required AnnotatedTag Tag { get; set; }
    public required List<Commit> Commits { get; set; }

    //TODO: Also get commit from annotatedTag.
}
