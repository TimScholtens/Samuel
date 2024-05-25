using FluentAssertions;

namespace Samuel.Domain.Tests;

[Trait("Category", "Unit")]
public class SemanticVersionTests
{
    [Fact]
    public void Increment_WhenFix_ShouldIncrementPatchVersion()
    {
        // Arrange
        var version = new SemanticVersion();

        // Act
        var result = version.Increment(CommitType.Fix);

        // Assert
        result.Major.Should().Be(1);
        result.Minor.Should().Be(0);
        result.Patch.Should().Be(1);
    }

    [Fact]
    public void Increment_WhenFeature_ShouldIncrementMinorVersion()
    {
        // Arrange
        var version = new SemanticVersion();

        // Act
        var result = version.Increment(CommitType.Feature);

        // Assert
        result.Major.Should().Be(1);
        result.Minor.Should().Be(1);
        result.Patch.Should().Be(0);
    }

    [Fact]
    public void Increment_WhenBreakingChange_ShouldIncrementMajorVersion()
    {
        // Arrange
        var version = new SemanticVersion();

        // Act
        var result = version.Increment(CommitType.Breaking);

        // Assert
        result.Major.Should().Be(2);
        result.Minor.Should().Be(0);
        result.Patch.Should().Be(0);
    }

    [Fact]
    public void Increment_WhenNone_ShouldThrowException()
    {
        // Arrange
        var version = new SemanticVersion();

        // Act
        var result = () => version.Increment(CommitType.None);

        // Assert
        result.Should().Throw<Exception>();
    }
}
