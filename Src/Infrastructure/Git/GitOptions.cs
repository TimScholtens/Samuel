namespace Samuel.Infrastructure.Git;

public class GitOptions
{
    public const string SectionName = "Git";
    public required string CommiterEmail { get; set; }
    public required string CommiterName { get; set; }
    public required string CommitMessageParseRegex { get; set; }
    public string Username { get; set; } = "User";
    public string? Password { get; set; }
    public string? Credentials { get; set; }
}
