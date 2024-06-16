using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Samuel.Application.Features.SemanticRelease;
using Samuel.Application.Features.SemanticRelease.Changelog;
using Samuel.Application.Features.SemanticRelease.Shared.Git;
using Samuel.Application.Features.SemanticRelease.Shared.Pipeline;
using Samuel.Domain;
using Samuel.Domain.Tests.Helpers;

namespace Samuel.Application.Tests.Features.Changelog;

[Trait("Category", "Unit")]
public class ChangelogGeneratorStepTests
{
    [Fact]
    public void Execute_WhenBreakingChange_ShouldGenerateChangelogFeatures()
    {
        // Arrange
        var options = Options.Create(new ChangelogGeneratorOptions()
        {
            OutputFilePath = "Changelog.md",
            Title = "Test",
            IssueUrlFormat = "https://dev.azure.com/ScholtensIO/NET-101/_workitems/edit/",
            FeatureSectionTitle = "*Features*",
            FixSectionTitle = "*Fixes*"
        });
        var gitService = A.Fake<IGitService>();
        var logger = A.Fake<ILogger<SemanticReleaser>>();
        var writer = A.Fake<IChangelogWriter>();
        var newCommit = new CommitBuilder()
            .WithType(CommitType.Breaking)
            .Build();
        var tag = new AnnotatedTagBuilder().Build();
        var context = new PipelineContext()
        {
            NewRelease = new ReleaseBuilder()
                .WithTag(tag)
                .WithCommit(newCommit)
                .Build()
        };
        var sut = new ChangelogGeneratorStep(options, gitService, logger, writer);

        A.CallTo(() => gitService.GetAllReleases()).Returns(new List<Release>() { });

        // Act
        sut.Execute(context, new PipelineCancellationToken());

        // Assert
        A.CallTo(() => writer.WriteTitle(options.Value.Title)).MustHaveHappenedOnceExactly();
        A.CallTo(() => writer.WriteHeader(context.NewRelease.Tag.Version.ToString())).MustHaveHappenedOnceExactly();
        A.CallTo(() => writer.WriteSubTitle(options.Value.FeatureSectionTitle)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Execute_WhenBreakingChangeAndOnePreviousRelease_ShouldGenerateChangelogFeatures()
    {
        // Arrange
        var options = Options.Create(new ChangelogGeneratorOptions()
        {
            OutputFilePath = "Changelog.md",
            Title = "Test",
            IssueUrlFormat = "https://dev.azure.com/ScholtensIO/NET-101/_workitems/edit/",
            FeatureSectionTitle = "*Features*",
            FixSectionTitle = "*Fixes*"
        });
        var gitService = A.Fake<IGitService>();
        var logger = A.Fake<ILogger<SemanticReleaser>>();
        var writer = A.Fake<IChangelogWriter>();
        var oldCommit = new CommitBuilder()
            .WithType(CommitType.Breaking)
            .Build();
        var newCommit = new CommitBuilder()
            .WithType(CommitType.Breaking)
            .Build();
        var oldTag = new AnnotatedTagBuilder()
            .WithName("tag-1")
            .WithVersion("1.0.0")
            .Build();
        var newTag = new AnnotatedTagBuilder()
            .WithName("tag-2")
            .WithVersion("2.0.0")
            .Build();
        var newRelease = new ReleaseBuilder()
            .WithTag(newTag)
            .WithCommit(newCommit)
            .Build();
        var oldRelease = new ReleaseBuilder()
            .WithTag(oldTag)
            .WithCommit(oldCommit)
            .Build();

        var context = new PipelineContext()
        {
            NewRelease = newRelease
        };
        var sut = new ChangelogGeneratorStep(options, gitService, logger, writer);

        A.CallTo(() => gitService.GetAllReleases()).Returns(new List<Release>() { oldRelease });

        // Act
        sut.Execute(context, new PipelineCancellationToken());

        // Assert
        A.CallTo(() => writer.WriteTitle(options.Value.Title)).MustHaveHappenedOnceExactly();
        A.CallTo(() => writer.WriteHeader(newRelease.Tag.Version.ToString())).MustHaveHappenedOnceExactly();
        A.CallTo(() => writer.WriteHeader(oldRelease.Tag.Version.ToString())).MustHaveHappenedOnceExactly();
        A.CallTo(() => writer.WriteSubTitle(options.Value.FeatureSectionTitle)).MustHaveHappenedTwiceExactly();
    }


    [Fact]
    public void Execute_WhenBreakingChangeAndFix_ShouldGenerateChangelogFeaturesAndFix()
    {
        // Arrange
        var options = Options.Create(new ChangelogGeneratorOptions()
        {
            OutputFilePath = "Changelog.md",
            Title = "Test",
            IssueUrlFormat = "https://dev.azure.com/ScholtensIO/NET-101/_workitems/edit/",
            FeatureSectionTitle = "*Features*",
            FixSectionTitle = "*Fixes*"
        });
        var gitService = A.Fake<IGitService>();
        var logger = A.Fake<ILogger<SemanticReleaser>>();
        var writer = A.Fake<IChangelogWriter>();
        var breakingCommit = new CommitBuilder()
            .WithType(CommitType.Breaking)
            .Build();
        var fixCommit = new CommitBuilder()
            .WithType(CommitType.Fix)
            .Build();
        var tag = new AnnotatedTagBuilder().Build();
        var context = new PipelineContext()
        {
            NewRelease = new ReleaseBuilder()
                .WithTag(tag)
                .WithCommit(breakingCommit)
                .WithCommit(fixCommit)
                .Build()
        };
        var sut = new ChangelogGeneratorStep(options, gitService, logger, writer);

        A.CallTo(() => gitService.GetAllReleases()).Returns(new List<Release>() { });

        // Act
        sut.Execute(context, new PipelineCancellationToken());

        // Assert
        A.CallTo(() => writer.WriteTitle(options.Value.Title)).MustHaveHappenedOnceExactly();
        A.CallTo(() => writer.WriteHeader(context.NewRelease.Tag.Version.ToString())).MustHaveHappenedOnceExactly();
        A.CallTo(() => writer.WriteSubTitle(options.Value.FeatureSectionTitle)).MustHaveHappenedOnceExactly();
        A.CallTo(() => writer.WriteSubTitle(options.Value.FixSectionTitle)).MustHaveHappenedOnceExactly();
    }
}
