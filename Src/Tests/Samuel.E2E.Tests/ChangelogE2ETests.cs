using FluentAssertions;
using Samuel.CLI;
using Samuel.E2E.Tests.Helpers;
using Samuel.Infrastructure.Tests.Helpers;

namespace Samuel.E2E.Tests;

[Trait("Category", "E2E")]
public class ChangelogE2ETests
{
    [Fact]
    public void GenerateChangelog_WhenNoCommits_ShouldNotGenerateChangelog()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), $"samuel-{Guid.NewGuid()}");

        new LibGitRepositoryBuilder(path).Build();

        Directory.SetCurrentDirectory(path);

        // Act
        var exitCode = Program.Main(["run", "--dry-run", "--debug"]);

        // Assert
        var changelogExists = File.Exists(Path.Combine(path, "CHANGELOG.md"));

        exitCode.Should().Be(0);
        changelogExists.Should().BeFalse();
    }


    [Fact]
    public void GenerateChangelog_WhenNoBigChange_ShouldNotGenerateChangelog()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), $"samuel-{Guid.NewGuid()}");

        new LibGitRepositoryBuilder(path)
            .WithCommit("no big change.")
            .Build();

        Directory.SetCurrentDirectory(path);

        // Act
        var exitCode = Program.Main(["run", "--dry-run", "--debug"]);

        // Assert
        var changelogExists = File.Exists(Path.Combine(path, "CHANGELOG.md"));

        exitCode.Should().Be(0);
        changelogExists.Should().BeFalse();
    }


    [Fact]
    public void GenerateChangelog_WhenOneBreakingChange_ShouldGenerateChangelogWithBreakingChange()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), $"samuel-{Guid.NewGuid()}");

        new LibGitRepositoryBuilder(path)
            .WithCommit("Merged PR 1: BREAKING: added caching")
            .Build();

        Directory.SetCurrentDirectory(path);

        // Act
        var exitCode = Program.Main(["run", "--dry-run", "--debug"]);

        // Assert
        var changelogContent = ChangelogReader.GetChangeLogContent(Path.Combine(path, "CHANGELOG.md"));

        exitCode.Should().Be(0);
        changelogContent.Should().Be($"# Changelog{Environment.NewLine}## 1.0.0{Environment.NewLine}*Features*{Environment.NewLine}- Merged PR 1: BREAKING: added caching, closes issue(s): .{Environment.NewLine}");
    }


}