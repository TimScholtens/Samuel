using FakeItEasy;
using FluentAssertions.Common;

namespace Samuel.Infrastructure.Tests.Helpers;

public class LibGitCommitBuilder
{
    private string _commitMessage;
    private string _commitId;
    private string _sha;
    private DateTime _createdAt;
    private string _authorName;
    private string _authorEmail;

    public LibGitCommitBuilder()
    {
        _commitId = "564289a5f34546401bbc727c20a14069761f9143";
        _sha = "564289a5f34546401bbc727c20a14069761f9143";
        _commitMessage = "wip";
        _authorName = "author1";
        _authorEmail = "author1@noreply.com";
        _createdAt = DateTime.UtcNow;
    }

    public LibGitCommitBuilder WithId(string id)
    {
        _commitId = id;
        return this;
    }

    public LibGitCommitBuilder WithMessage(string message)
    {
        _commitMessage = message;
        return this;
    }

    public LibGitCommitBuilder WithAuthorName(string name)
    {
        _authorName = name;
        return this;
    }

    public LibGitCommitBuilder WithAuthorEmail(string mail)
    {
        _authorEmail = mail;
        return this;
    }

    public LibGitCommitBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public LibGit2Sharp.Commit Build()
    {
        var commit = A.Fake<LibGit2Sharp.Commit>();

        A.CallTo(() => commit.Message).Returns(_commitMessage);
        A.CallTo(() => commit.Id).Returns(new LibGit2Sharp.ObjectId(_commitId));
        A.CallTo(() => commit.Sha).Returns(_sha);
        A.CallTo(() => commit.Author).Returns(new LibGit2Sharp.Signature(_authorName, _authorEmail, _createdAt.ToDateTimeOffset()));
        A.CallTo(() => commit.Committer).Returns(new LibGit2Sharp.Signature(_authorName, _authorEmail, _createdAt.ToDateTimeOffset()));

        return commit;
    }
}


