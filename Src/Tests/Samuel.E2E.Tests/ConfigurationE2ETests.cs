using FluentAssertions;
using Samuel.CLI;
using Samuel.E2E.Tests.Helpers;
using Samuel.Infrastructure.Tests.Helpers;
using System.Text.Json;

namespace Samuel.E2E.Tests;

[Trait("Category", "E2E")]
public class ConfigurationE2ETests
{
    [Fact]
    public void OverrideConfiguration_WhenDifferentChangelogTitle_ShouldUseDifferentChangelogTitle()
    {
        // Arrange
        var path = Path.Combine(Path.GetTempPath(), $"samuel-{Guid.NewGuid()}");

        new LibGitRepositoryBuilder(path)
            .WithCommit("Merged PR 1: BREAKING: added caching")
            .Build();

        Directory.SetCurrentDirectory(path);

        var configurationOptions = new { ChangelogGenerator = new { Title = "NewChangelogTitle" } };
        var jsonConfigurationOptions = JsonSerializer.Serialize(configurationOptions);

        CreateConfigurationFile(path, jsonConfigurationOptions);

        // Act
        var exitCode = Program.Main(["run", "--dry-run", "--debug"]);

        // Assert
        var changelogContent = ChangelogReader.GetChangeLogContent(Path.Combine(path, "CHANGELOG.md"));

        exitCode.Should().Be(0);
        changelogContent.Should().Be($"# NewChangelogTitle{Environment.NewLine}## 1.0.0{Environment.NewLine}*Features*{Environment.NewLine}- Merged PR 1: BREAKING: added caching, closes issue(s): .{Environment.NewLine}");
    }

    private void CreateConfigurationFile(string directory, string configuration)
    {
        var configurationFilePath = Path.Combine(directory, "Configuration.json");

        File.WriteAllText(configurationFilePath, string.Join(Environment.NewLine, configuration));

    }
}
