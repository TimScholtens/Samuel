namespace Samuel.Domain.Tests.Helpers;

public class CommitBuilder
{
    private string _id;
    private DateTime _createdAt;
    private string _description;
    private string _message;
    private string[] _relatedWorkItems;
    private string _title;
    private CommitType _type;
    private string _sha;

    public CommitBuilder()
    {
        _id = "1";
        _sha = "1";
        _createdAt = DateTime.Now;
        _description = string.Empty;
        _message = string.Empty;
        _relatedWorkItems = new string[] { "1", "2" };
        _title = string.Empty;
        _type = CommitType.Feature;
    }

    public CommitBuilder WithType(CommitType type)
    {
        _type = type;
        return this;
    }

    public CommitBuilder WithMessage(string message)
    {
        _message = message;
        return this;
    }

    public Commit Build()
    {
        return new Commit()
        {
            Id = _id,
            Sha = _sha,
            CreatedAt = _createdAt,
            Description = _description,
            RawContent = _message,
            RelatedWorkItemsIds = _relatedWorkItems,
            Title = _title,
            Type = _type
        };
    }
}
