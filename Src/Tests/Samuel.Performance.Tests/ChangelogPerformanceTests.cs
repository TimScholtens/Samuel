using BenchmarkDotNet.Attributes;
using Samuel.Infrastructure.Tests.Helpers;

namespace Samuel.Performance.Tests;


public class ChangelogPerformanceTests
{

    #region No big change
    [GlobalSetup(Target = nameof(GenerateChangelog_WhenNoBigChange))]
    public void SetupNoBigChange()
    {
        var path = Path.Combine(Path.GetTempPath(), $"samuel-{Guid.NewGuid()}");

        new LibGitRepositoryBuilder(path)
            .WithCommit("no big change.")
            .Build();

        Directory.SetCurrentDirectory(path);
    }


    [Benchmark]
    public void GenerateChangelog_WhenNoBigChange()
    {
        Samuel.CLI.Program.Main(["run", "--dry-run"]);
    }
    #endregion

    #region One big change

    [GlobalSetup(Target = nameof(GenerateChangelog_WhenOneBigChange))]
    public void SetupOneBigChange()
    {
        var path = Path.Combine(Path.GetTempPath(), $"samuel-{Guid.NewGuid()}");

        new LibGitRepositoryBuilder(path)
            .WithCommit("Merged PR 1: BREAKING: added caching")
            .Build();

        Directory.SetCurrentDirectory(path);
    }

    [Benchmark]
    public void GenerateChangelog_WhenOneBigChange()
    {
        // Act
        Samuel.CLI.Program.Main(["run", "--dry-run"]);
    }
    #endregion

    #region With 1000 changes

    [GlobalSetup(Target = nameof(GenerateChangelog_When1000BigChanges))]
    public void SetupWhen1000BigChanges()
    {
        var path = Path.Combine(Path.GetTempPath(), $"samuel-{Guid.NewGuid()}");

        new LibGitRepositoryBuilder(path)
            .WithManyCommits(1000, "Merged PR 1: BREAKING: added caching")
            .Build();

        Directory.SetCurrentDirectory(path);
    }


    [Benchmark]
    public void GenerateChangelog_When1000BigChanges()
    {
        Samuel.CLI.Program.Main(["run", "--dry-run"]);
    }

    #endregion

    #region With 10000 changes

    [GlobalSetup(Target = nameof(GenerateChangelog_When10000BigChanges))]
    public void SetupWhen10000BigChanges()
    {
        var path = Path.Combine(Path.GetTempPath(), $"samuel-{Guid.NewGuid()}");

        new LibGitRepositoryBuilder(path)
            .WithManyCommits(10000, "Merged PR 1: BREAKING: added caching")
            .Build();

        Directory.SetCurrentDirectory(path);
    }


    [Benchmark]
    public void GenerateChangelog_When10000BigChanges()
    {
        Samuel.CLI.Program.Main(["run", "--dry-run"]);
    }

    #endregion

}