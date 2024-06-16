using FluentAssertions;
using Samuel.CLI;
using Samuel.E2E.Tests.Helpers;
using Samuel.Infrastructure.Tests.Helpers;
using System.Globalization;

namespace Samuel.E2E.Tests;

[Trait("Category", "E2E")]
[Collection("Sequential")]
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

        var createdAtDate = new DateTime(2024, 6, 16, 0, 0, 0, DateTimeKind.Utc);
        new LibGitRepositoryBuilder(path)
            .WithCommit("Merged PR 1: BREAKING: added caching", createdAtDate)
            .Build();

        Directory.SetCurrentDirectory(path);

        // Act
        var exitCode = Program.Main(["run", "--dry-run", "--debug"]);

        // Assert
        var changelogContent = ChangelogReader.GetChangeLogContent(Path.Combine(path, "CHANGELOG.md"));

        exitCode.Should().Be(0);
        changelogContent.Should().Be(string.Join(Environment.NewLine,
                                                "# Changelog",
                                                $"## 1.0.0 {DateOnly.FromDateTime(createdAtDate).ToString("dd-M-yyyy", CultureInfo.InvariantCulture)}",
                                                "*:star: Features*",
                                                $"- Merged PR 1: BREAKING: added caching{Environment.NewLine}{Environment.NewLine}"));
    }

    [Fact]
    public void GenerateChangelog_WhenOneBreakingChangeAndLinkedIssue_ShouldGenerateChangelogWithBreakingChangeAndLinkedIssue()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), $"samuel-{Guid.NewGuid()}");

        var createdAtDate = new DateTime(2024, 6, 16, 0, 0, 0, DateTimeKind.Utc);
        new LibGitRepositoryBuilder(path)
            .WithCommit($"Merged PR 1: BREAKING: added caching{Environment.NewLine}{Environment.NewLine}wip{Environment.NewLine}{Environment.NewLine}Related work items: #22, #23", createdAtDate)
            .Build();

        Directory.SetCurrentDirectory(path);

        // Act
        var exitCode = Program.Main(["run", "--dry-run", "--debug"]);

        // Assert
        var changelogContent = ChangelogReader.GetChangeLogContent(Path.Combine(path, "CHANGELOG.md"));

        exitCode.Should().Be(0);
        changelogContent.Should().Be(string.Join(Environment.NewLine,
                                        "# Changelog",
                                        $"## 1.0.0 {DateOnly.FromDateTime(createdAtDate).ToString("dd-M-yyyy", CultureInfo.InvariantCulture)}",
                                        "*:star: Features*",
                                        $"- Merged PR 1: BREAKING: added caching, closes issue(s): [22](https://dev.azure.com/ScholtensIO/NET-101/_workitems/edit/22),[23](https://dev.azure.com/ScholtensIO/NET-101/_workitems/edit/23).{Environment.NewLine}{Environment.NewLine}"));
    }

    [Fact]
    public void GenerateChangelog_WhenTwoBreakingChanges_ShouldGenerateChangelogWithBreakingChanges()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), $"samuel-{Guid.NewGuid()}");

        var createdFirstCommitAtDate = new DateTime(2024, 6, 16, 0, 0, 0, DateTimeKind.Utc);
        var createdSecondCommitAtDate = new DateTime(2024, 6, 17, 0, 0, 0, DateTimeKind.Utc);
        new LibGitRepositoryBuilder(path)
            .WithCommit($"Merged PR 1: BREAKING: added caching{Environment.NewLine}{Environment.NewLine}wip{Environment.NewLine}{Environment.NewLine}Related work items: #22, #23", createdFirstCommitAtDate)
            .WithCommit($"Merged PR 2: BREAKING: added logging{Environment.NewLine}{Environment.NewLine}wip{Environment.NewLine}{Environment.NewLine}Related work items: #24", createdSecondCommitAtDate)
            .Build();

        Directory.SetCurrentDirectory(path);

        // Act
        var exitCode = Program.Main(["run", "--dry-run", "--debug"]);

        // Assert
        var changelogContent = ChangelogReader.GetChangeLogContent(Path.Combine(path, "CHANGELOG.md"));

        exitCode.Should().Be(0);
        changelogContent.Should().Be(string.Join(Environment.NewLine,
                                        "# Changelog",
                                        $"## 1.0.0 {DateOnly.FromDateTime(createdSecondCommitAtDate).ToString("dd-M-yyyy", CultureInfo.InvariantCulture)}",
                                        "*:star: Features*",
                                        "- Merged PR 2: BREAKING: added logging, closes issue(s): [24](https://dev.azure.com/ScholtensIO/NET-101/_workitems/edit/24).",
                                        $"- Merged PR 1: BREAKING: added caching, closes issue(s): [22](https://dev.azure.com/ScholtensIO/NET-101/_workitems/edit/22),[23](https://dev.azure.com/ScholtensIO/NET-101/_workitems/edit/23).{Environment.NewLine}{Environment.NewLine}"));

    }

    [Fact]
    public void GenerateChangelog_WhenPreviousRelease_ShouldGenerateChangelogWithTwoReleases()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), $"samuel-{Guid.NewGuid()}");

        var createdFirstCommitAtDate = new DateTime(2024, 6, 16, 0, 0, 0, DateTimeKind.Utc);
        var createdSecondCommitAtDate = new DateTime(2024, 6, 17, 0, 0, 0, DateTimeKind.Utc);
        new LibGitRepositoryBuilder(path)
            .WithCommit($"Merged PR 1: BREAKING: added caching{Environment.NewLine}{Environment.NewLine}wip{Environment.NewLine}{Environment.NewLine}Related work items: #22, #23", createdFirstCommitAtDate)
            .WithTag("1.0.0")
            .WithCommit($"Merged PR 2: BREAKING: added logging{Environment.NewLine}{Environment.NewLine}wip{Environment.NewLine}{Environment.NewLine}Related work items: #24", createdSecondCommitAtDate)
            .Build();

        Directory.SetCurrentDirectory(path);

        // Act
        var exitCode = Program.Main(["run", "--dry-run", "--debug"]);

        // Assert
        var changelogContent = ChangelogReader.GetChangeLogContent(Path.Combine(path, "CHANGELOG.md"));

        exitCode.Should().Be(0);
        changelogContent.Should().Be(string.Join(Environment.NewLine,
                                                 "# Changelog",
                                                 $"## 2.0.0 {DateOnly.FromDateTime(createdSecondCommitAtDate).ToString("dd-M-yyyy", CultureInfo.InvariantCulture)}",
                                                 "*:star: Features*",
                                                 $"- Merged PR 2: BREAKING: added logging, closes issue(s): [24](https://dev.azure.com/ScholtensIO/NET-101/_workitems/edit/24).{Environment.NewLine}",
                                                 $"## 1.0.0 {DateOnly.FromDateTime(createdFirstCommitAtDate).ToString("dd-M-yyyy", CultureInfo.InvariantCulture)}",
                                                 "*:star: Features*",
                                                 $"- Merged PR 1: BREAKING: added caching, closes issue(s): [22](https://dev.azure.com/ScholtensIO/NET-101/_workitems/edit/22),[23](https://dev.azure.com/ScholtensIO/NET-101/_workitems/edit/23).{Environment.NewLine}{Environment.NewLine}"));
    }

    [Fact]
    public void GenerateChangelog_WhenPreviousReleaseAndFix_ShouldGenerateChangelogWithTwoReleasesContainingFix()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), $"samuel-{Guid.NewGuid()}");

        var createdFirstCommitAtDate = new DateTime(2024, 6, 16, 0, 0, 0, DateTimeKind.Utc);
        var createdSecondCommitAtDate = new DateTime(2024, 6, 17, 0, 0, 0, DateTimeKind.Utc);
        var createdThirdCommitAtDate = new DateTime(2024, 6, 18, 0, 0, 0, DateTimeKind.Utc);

        new LibGitRepositoryBuilder(path)
            .WithCommit($"Merged PR 1: Fix: fixed caching{Environment.NewLine}{Environment.NewLine}wip{Environment.NewLine}{Environment.NewLine}Related work items: #24", createdFirstCommitAtDate)
            .WithCommit($"Merged PR 2: BREAKING: added caching{Environment.NewLine}{Environment.NewLine}wip{Environment.NewLine}{Environment.NewLine}Related work items: #22, #23", createdSecondCommitAtDate)
            .WithTag("1.0.0")
            .WithCommit($"Merged PR 3: BREAKING: added logging{Environment.NewLine}{Environment.NewLine}wip{Environment.NewLine}{Environment.NewLine}Related work items: #25", createdThirdCommitAtDate)
            .Build();

        Directory.SetCurrentDirectory(path);

        // Act
        var exitCode = Program.Main(["run", "--dry-run", "--debug"]);

        // Assert
        var changelogContent = ChangelogReader.GetChangeLogContent(Path.Combine(path, "CHANGELOG.md"));

        exitCode.Should().Be(0);
        changelogContent.Should().Be(string.Join(Environment.NewLine,
                                                 "# Changelog",
                                                 $"## 2.0.0 {DateOnly.FromDateTime(createdThirdCommitAtDate).ToString("dd-M-yyyy", CultureInfo.InvariantCulture)}",
                                                 "*:star: Features*",
                                                 $"- Merged PR 3: BREAKING: added logging, closes issue(s): [25](https://dev.azure.com/ScholtensIO/NET-101/_workitems/edit/25).{Environment.NewLine}",
                                                 $"## 1.0.0 {DateOnly.FromDateTime(createdSecondCommitAtDate).ToString("dd-M-yyyy", CultureInfo.InvariantCulture)}",
                                                 "*:star: Features*",
                                                 $"- Merged PR 2: BREAKING: added caching, closes issue(s): [22](https://dev.azure.com/ScholtensIO/NET-101/_workitems/edit/22),[23](https://dev.azure.com/ScholtensIO/NET-101/_workitems/edit/23).{Environment.NewLine}",
                                                 $"*:bug: Fixes*",
                                                 $"- Merged PR 1: Fix: fixed caching, closes issue(s): [24](https://dev.azure.com/ScholtensIO/NET-101/_workitems/edit/24).{Environment.NewLine}{Environment.NewLine}"));
    }
}