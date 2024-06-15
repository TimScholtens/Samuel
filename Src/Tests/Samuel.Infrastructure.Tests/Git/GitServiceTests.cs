using FakeItEasy;
using FluentAssertions;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Samuel.Domain.Tests.Helpers;
using Samuel.Infrastructure.Git;
using Samuel.Infrastructure.Tests.Helpers;

namespace Samuel.Infrastructure.Tests.Git;

[Trait("Category", "Integration")]
public class GitServiceTests
{
    #region GetCommits
    [Fact]
    public void GetCommits_WhenNone_ShouldReturnEmptyList()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });
        var mapper = A.Fake<IGitServiceMapper>();
        var repo = new LibGitRepositoryBuilder().Build();
        var repoFactory = A.Fake<IRepositoryFactory>();
        var logger = A.Fake<ILogger<GitService>>();

        A.CallTo(() => repoFactory.Create()).Returns(repo);

        var sut = new GitService(options, mapper, repoFactory, logger);

        // Act
        var commits = sut.GetCommits();

        // Assert
        commits.Should().BeEmpty();
    }

    [Fact]
    public void GetCommits_WhenOne_ShouldReturnNonEmptyList()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });
        var mapper = A.Fake<IGitServiceMapper>();
        var repoFactory = A.Fake<IRepositoryFactory>();
        var repo = new LibGitRepositoryBuilder()
            .WithCommit("Message-1")
            .Build();
        var logger = A.Fake<ILogger<GitService>>();

        A.CallTo(() => repoFactory.Create()).Returns(repo);

        var sut = new GitService(options, mapper, repoFactory, logger);

        // Act
        var commits = sut.GetCommits();

        // Assert
        commits.Should().HaveCount(1);
    }
    #endregion

    #region GetCommitsSince
    [Fact]
    public void GetCommitsSince_WhenNone_ShouldReturnEmptyList()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });
        var mapper = A.Fake<IGitServiceMapper>();
        var repoFactory = A.Fake<IRepositoryFactory>();
        var repo = new LibGitRepositoryBuilder()
            .WithCommit("Message-1")
            .Build();
        var logger = A.Fake<ILogger<GitService>>();

        A.CallTo(() => repoFactory.Create()).Returns(repo);

        var sut = new GitService(options, mapper, repoFactory, logger);

        // Act
        var commits = sut.GetCommits();

        // Assert
        commits.Should().HaveCount(1);
    }

    [Fact]
    public void GetCommitsSince_WhenOneAfter_ShouldReturnOneCommit()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });
        var mapper = A.Fake<IGitServiceMapper>();
        var repoFactory = A.Fake<IRepositoryFactory>();
        var repo = new LibGitRepositoryBuilder()
                .WithCommit("message-1")
                .WithCommit("message-2")
            .Build();
        var logger = A.Fake<ILogger<GitService>>();

        A.CallTo(() => repoFactory.Create()).Returns(repo);

        var sut = new GitService(options, mapper, repoFactory, logger);

        // Act
        var commits = sut.GetCommitsAfter(repo.Commits.Last().Sha);

        // Assert
        commits.Should().HaveCount(1);
    }

    [Fact]
    public void GetCommitsSince_WhenTwoAfter_ShouldReturnTwoCommits()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });
        var mapper = A.Fake<IGitServiceMapper>();
        var repoFactory = A.Fake<IRepositoryFactory>();
        var repo = new LibGitRepositoryBuilder()
                .WithCommit("message-1")
                .WithCommit("message-2")
                .WithCommit("message-3")
            .Build();
        var logger = A.Fake<ILogger<GitService>>();

        A.CallTo(() => repoFactory.Create()).Returns(repo);

        var sut = new GitService(options, mapper, repoFactory, logger);

        // Act
        var commits = sut.GetCommitsAfter(repo.Commits.Last().Sha);

        // Assert
        commits.Should().HaveCount(2);
    }

    [Fact]
    public void GetCommitsSince_WhenOneBefore_ShouldReturnEmptyList()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });
        var mapper = A.Fake<IGitServiceMapper>();
        var repoFactory = A.Fake<IRepositoryFactory>();
        var repo = new LibGitRepositoryBuilder()
            .WithCommit("Message-1")
            .Build();
        var logger = A.Fake<ILogger<GitService>>();

        A.CallTo(() => repoFactory.Create()).Returns(repo);

        var sut = new GitService(options, mapper, repoFactory, logger);

        // Act
        var commits = sut.GetCommitsAfter(repo.Commits.Last().Sha);

        // Assert
        commits.Should().HaveCount(0);
    }

    #endregion

    #region GetCommitsBetween
    [Fact]
    public void GetCommitsBetween_WhenOneInMiddle_ShouldReturnOne()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });
        var mapper = A.Fake<IGitServiceMapper>();
        var repoFactory = A.Fake<IRepositoryFactory>();
        var repo = new LibGitRepositoryBuilder()
            .WithCommit("Message-1")
            .WithCommit("Message-2")
            .WithCommit("Message-3")
            .Build();
        var logger = A.Fake<ILogger<GitService>>();

        A.CallTo(() => repoFactory.Create()).Returns(repo);

        var sut = new GitService(options, mapper, repoFactory, logger);

        // Act
        var firstCommit = repo.Commits.Last();
        var lastCommit = repo.Commits.First();

        var commits = sut.GetCommitsBetween(firstCommit.Sha, lastCommit.Sha);

        // Assert
        commits.Should().HaveCount(1);
    }

    [Fact]
    public void GetCommitsBetween_WhenTwoInMiddle_ShouldReturnTwo()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });
        var mapper = A.Fake<IGitServiceMapper>();
        var repoFactory = A.Fake<IRepositoryFactory>();
        var repo = new LibGitRepositoryBuilder()
            .WithCommit("Message-1")
            .WithCommit("Message-2")
            .WithCommit("Message-3")
            .WithCommit("Message-4")
            .Build();
        var logger = A.Fake<ILogger<GitService>>();

        A.CallTo(() => repoFactory.Create()).Returns(repo);

        var sut = new GitService(options, mapper, repoFactory, logger);

        // Act
        var firstCommit = repo.Commits.Last();
        var lastCommit = repo.Commits.First();

        var commits = sut.GetCommitsBetween(firstCommit.Sha, lastCommit.Sha);

        // Assert
        commits.Should().HaveCount(2);
    }

    [Fact]
    public void GetCommitsBetween_WhenOneInMiddleAndOneAfter_ShouldReturnOne()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });
        var mapper = A.Fake<IGitServiceMapper>();
        var repoFactory = A.Fake<IRepositoryFactory>();
        var repo = new LibGitRepositoryBuilder()
            .WithCommit("Message-1")
            .WithCommit("Message-2")
            .WithCommit("Message-3")
            .WithCommit("Message-4")
            .Build();
        var logger = A.Fake<ILogger<GitService>>();

        A.CallTo(() => repoFactory.Create()).Returns(repo);

        var sut = new GitService(options, mapper, repoFactory, logger);

        // Act
        var firstCommit = repo.Commits.Last();
        var secondLastCommit = repo.Commits.ElementAt(1);

        var commits = sut.GetCommitsBetween(firstCommit.Sha, secondLastCommit.Sha);

        // Assert
        commits.Should().HaveCount(1);
    }

    [Fact]
    public void GetCommitsBetween_WhenOneInMiddleAndOneBefore_ShouldReturnOne()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });
        var mapper = A.Fake<IGitServiceMapper>();
        var repoFactory = A.Fake<IRepositoryFactory>();
        var repo = new LibGitRepositoryBuilder()
            .WithCommit("Message-1")
            .WithCommit("Message-2")
            .WithCommit("Message-3")
            .WithCommit("Message-4")
            .Build();
        var logger = A.Fake<ILogger<GitService>>();

        A.CallTo(() => repoFactory.Create()).Returns(repo);

        var sut = new GitService(options, mapper, repoFactory, logger);

        // Act
        var secondCommit = repo.Commits.ToList().ElementAt(repo.Commits.Count() - 2);
        var lastCommit = repo.Commits.First();

        var commits = sut.GetCommitsBetween(secondCommit.Sha, lastCommit.Sha);

        // Assert
        commits.Should().HaveCount(1);
    }

    [Fact]
    public void GetCommitsBetween_WhenNoneInMiddle_ShouldReturnEmptyList()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });
        var mapper = A.Fake<IGitServiceMapper>();
        var repoFactory = A.Fake<IRepositoryFactory>();
        var repo = new LibGitRepositoryBuilder()
            .WithCommit("Message-1")
            .WithCommit("Message-2")
            .Build();
        var logger = A.Fake<ILogger<GitService>>();

        A.CallTo(() => repoFactory.Create()).Returns(repo);

        var sut = new GitService(options, mapper, repoFactory, logger);

        // Act
        var firstCommit = repo.Commits.Last();
        var lastCommit = repo.Commits.First();

        var commits = sut.GetCommitsBetween(firstCommit.Sha, lastCommit.Sha);

        // Assert
        commits.Should().HaveCount(0);
    }
    #endregion

    #region GetLatestRelease
    [Fact]
    public void GetLatestRelease_WhenNone_ShouldReturnEmptyList()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });
        var mapper = A.Fake<IGitServiceMapper>();
        var repoFactory = A.Fake<IRepositoryFactory>();
        var repo = new LibGitRepositoryBuilder().Build();
        var logger = A.Fake<ILogger<GitService>>();
        var sut = new GitService(options, mapper, repoFactory, logger);

        A.CallTo(() => repoFactory.Create()).Returns(repo);

        // Act
        var release = sut.GetLatestRelease();

        // Assert
        release.Should().Be(null);
    }


    [Fact]
    public void GetLatestRelease_WhenOne_ShouldReturnOne()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });
        var mapper = A.Fake<IGitServiceMapper>();
        var repo = new LibGitRepositoryBuilder()
            .WithCommit("message-1")
            .WithTag("1.0.0")
            .Build();
        var repoFactory = A.Fake<IRepositoryFactory>();
        var logger = A.Fake<ILogger<GitService>>();

        A.CallTo(() => mapper.Map(A<Tag>._, A<Domain.Commit>._)).Returns(new Domain.AnnotatedTag()
        {
            Version = new Domain.SemanticVersion(1, 0, 0),
            Name = "1.0.0",
            Commit = new CommitBuilder().Build()
        });

        A.CallTo(() => repoFactory.Create()).Returns(repo);

        var sut = new GitService(options, mapper, repoFactory, logger);

        // Act
        var release = sut.GetLatestRelease();

        // Assert
        release!.Tag!.Name.Should().Be("1.0.0");
    }

    [Fact]
    public void GetLatestRelease_WhenTwo_ShouldReturnOne()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });
        var mapper = new GitServiceMapper(options);
        var repoFactory = A.Fake<IRepositoryFactory>();
        var repo = new LibGitRepositoryBuilder()
            .WithCommit("message-1")
            .WithTag("1.0.0")
            .WithCommit("message-2")
            .WithTag("2.0.0")
            .Build();
        var logger = A.Fake<ILogger<GitService>>();
        A.CallTo(() => repoFactory.Create()).Returns(repo);

        var sut = new GitService(options, mapper, repoFactory, logger);

        // Act
        var release = sut.GetLatestRelease();

        // Assert
        release!.Tag!.Name.Should().Be("2.0.0");
    }
    #endregion

    #region GetAllReleases
    [Fact]
    public void GetAllReleases_WhenNone_ShouldReturnEmptyList()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });
        var mapper = new GitServiceMapper(options);
        var repo = new LibGitRepositoryBuilder().Build();
        var repoFactory = A.Fake<IRepositoryFactory>();
        var logger = A.Fake<ILogger<GitService>>();

        A.CallTo(() => repoFactory.Create()).Returns(repo);

        var sut = new GitService(options, mapper, repoFactory, logger);

        // Act
        var releases = sut.GetAllReleases();

        // Assert
        releases.Should().BeEmpty();
    }

    [Fact]
    public void GetAllReleases_WhenOne_ShouldReturnOne()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });
        var mapper = new GitServiceMapper(options);
        var repoFactory = A.Fake<IRepositoryFactory>();
        var repo = new LibGitRepositoryBuilder()
            .WithCommit("message-1")
            .WithTag("1.0.0")
            .Build();
        var logger = A.Fake<ILogger<GitService>>();

        A.CallTo(() => repoFactory.Create()).Returns(repo);

        var sut = new GitService(options, mapper, repoFactory, logger);

        // Act
        var releases = sut.GetAllReleases();

        // Assert
        releases.Should().HaveCount(1);
    }

    [Fact]
    public void GetAllReleases_WhenTwo_ShouldReturnTwo()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });
        var mapper = new GitServiceMapper(options);
        var repoFactory = A.Fake<IRepositoryFactory>();
        var repo = new LibGitRepositoryBuilder()
            .WithCommit("message-1")
            .WithTag("1.0.0")
            .WithCommit("message-2")
            .WithTag("2.0.0")
            .Build();
        var logger = A.Fake<ILogger<GitService>>();

        A.CallTo(() => repoFactory.Create()).Returns(repo);

        var sut = new GitService(options, mapper, repoFactory, logger);

        // Act
        var releases = sut.GetAllReleases();

        // Assert
        releases.Should().HaveCount(2);
    }
    #endregion
}