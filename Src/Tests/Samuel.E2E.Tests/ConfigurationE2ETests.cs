using FluentAssertions;
using Samuel.CLI;
using Samuel.E2E.Tests.Helpers;
using Samuel.Infrastructure.Tests.Helpers;
using System.Globalization;
using System.Text.Json;

namespace Samuel.E2E.Tests;

[Trait("Category", "E2E")]
[Collection("Sequential")]
public class ConfigurationE2ETests
{
    [Fact]
    public void OverrideConfiguration_WhenDifferentChangelogTitle_ShouldUseDifferentChangelogTitle()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), $"samuel-{Guid.NewGuid()}");
        var createdCommitAtDate = new DateTime(2024, 6, 16, 0, 0, 0, DateTimeKind.Utc);

        new LibGitRepositoryBuilder(path)
            .WithCommit("Merged PR 1: BREAKING: added caching", createdCommitAtDate)
            .Build();

        Directory.SetCurrentDirectory(path);

        var configurationOptions = new { ChangelogGenerator = new { Title = "NewChangelogTitle", FeaturesSectionTitle = ":star: Features", FixesSectionTitle = ":bug: Fixes" } };
        var jsonConfigurationOptions = JsonSerializer.Serialize(configurationOptions);

        CreateConfigurationFile(path, jsonConfigurationOptions);

        // Act
        var exitCode = Program.Main(["run", "--dry-run", "--debug"]);

        // Assert
        var changelogContent = ChangelogReader.GetChangeLogContent(Path.Combine(path, "CHANGELOG.md"));

        exitCode.Should().Be(0);
        changelogContent.Should().Be(string.Join(Environment.NewLine,
                                        $"# {configurationOptions.ChangelogGenerator.Title}",
                                        $"## 1.0.0 {DateOnly.FromDateTime(createdCommitAtDate).ToString("dd-M-yyyy", CultureInfo.InvariantCulture)}",
                                        $"*{configurationOptions.ChangelogGenerator.FeaturesSectionTitle}*",
                                        $"- Merged PR 1: BREAKING: added caching{Environment.NewLine}{Environment.NewLine}"));
    }

    private void CreateConfigurationFile(string directory, string configuration)
    {
        var configurationFilePath = Path.Combine(directory, "Configuration.json");

        File.WriteAllText(configurationFilePath, string.Join(Environment.NewLine, configuration));

    }
}
