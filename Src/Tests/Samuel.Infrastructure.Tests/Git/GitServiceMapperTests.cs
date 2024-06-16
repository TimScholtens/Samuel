using FluentAssertions;
using Microsoft.Extensions.Options;
using Samuel.Infrastructure.Git;
using Samuel.Infrastructure.Tests.Helpers;

namespace Samuel.Infrastructure.Tests.Git;

[Trait("Category", "Unit")]
public class GitServiceMapperTests
{
    [Theory]
    [InlineData("wip")]
    [InlineData("wip wip")]
    public void Map_WhenDescriptionOnly_ShouldReturnDomainCommit(string description)
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });
        var commit = new LibGitCommitBuilder()
            .WithAuthorEmail(options.Value.CommiterEmail)
            .WithAuthorName(options.Value.CommiterName)
            .WithMessage(description)
            .WithCreatedAt(DateTime.UtcNow)
            .Build();
        var sut = new GitServiceMapper(options);

        // Act
        var result = sut.Map(commit);

        // Assert
        result.RawContent.Should().Be(description);
        result.CreatedAt.Should().Be(DateOnly.FromDateTime(commit.Author.When.UtcDateTime));
    }

    [Theory]
    [InlineData("    Merged PR 154: Feature | Add  caching\r\n\r\n    wip\r\n\r\n    Related work items: #22", "wip", new[] { "22" })]
    [InlineData("    Merged PR 155: Feature: added more space\r\n\r\n    wip\r\n\r\n    Related work items: #22, #23", "wip", new[] { "22", "23" })]
    public void Map_WhenMessageAndWorkItems_ShouldReturnDomainCommit(string message, string description, string[] workItemsIds)
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });

        var commit = new LibGitCommitBuilder()
            .WithAuthorEmail(options.Value.CommiterEmail)
            .WithAuthorName(options.Value.CommiterName)
            .WithMessage(message)
            .WithCreatedAt(DateTime.UtcNow)
            .Build();
        var sut = new GitServiceMapper(options);

        // Act
        var result = sut.Map(commit);

        // Assert
        result.Description.Should().Be(description);
        result.RelatedWorkItemsIds.Should().BeEquivalentTo(workItemsIds);
        result.CreatedAt.Should().Be(DateOnly.FromDateTime(commit.Author.When.UtcDateTime));
    }


    [Fact]
    public void ClassifyCommit_WhenNoMatch_ShouldReturnCommitTypeNone()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });

        var commitMessage = "edited something.";
        var sut = new GitServiceMapper(options);

        // Act
        var result = sut.ClassifyCommit(commitMessage);

        // Assert
        result.Should().Be(Domain.CommitType.None);
    }

    [Fact]
    public void ClassifyCommit_WhenMatchFeature_ShouldReturnCommitTypeFeature()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });

        var commitMessage = "Merged PR 1: feature: hello world ";
        var sut = new GitServiceMapper(options);

        // Act
        var result = sut.ClassifyCommit(commitMessage);

        // Assert
        result.Should().Be(Domain.CommitType.Feature);
    }

    [Fact]
    public void ClassifyCommit_WhenMatchBreaking_ShouldReturnCommitTypeBreaking()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });

        var commitMessage = "Merged PR 1: breaking: hello world ";
        var sut = new GitServiceMapper(options);

        // Act
        var result = sut.ClassifyCommit(commitMessage);

        // Assert
        result.Should().Be(Domain.CommitType.Breaking);
    }

    [Fact]
    public void RetrieveWorkItems_WhenNull_ShouldReturnEmptyList()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });

        string? workItems = null;
        var sut = new GitServiceMapper(options);

        // Act
        var result = sut.RetrieveWorkItems(workItems);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void RetrieveWorkItems_WhenNoMatch_ShouldReturnEmptyList()
    {
        // Arrange
        var options = Options.Create(new GitOptions()
        {
            CommiterEmail = "ChangelogBot",
            CommiterName = "ChangelogBot@noreply.com",
            CommitMessageParseRegex = "^Merged PR (\\d+): (\\w*): (.*)"
        });

        string? workItems = "abc";
        var sut = new GitServiceMapper(options);

        // Act
        var result = sut.RetrieveWorkItems(workItems);

        // Assert
        result.Should().BeEmpty();
    }
}
