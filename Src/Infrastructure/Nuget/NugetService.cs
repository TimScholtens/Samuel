using NuGet.Common;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using Samuel.Application.Features.SemanticRelease.ApplicationVersionChecker;

namespace Samuel.Infrastructure.Nuget;

public class NugetService : INugetService
{
    public Version? GetLatestNugetVersion(string packageName)
    {
        var cache = new SourceCacheContext();
        var repository = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
        var resource = repository.GetResourceAsync<FindPackageByIdResource>().Result;

        var versions = resource.GetAllVersionsAsync(packageName, cache, NullLogger.Instance, CancellationToken.None).Result;

        if (!versions.Any())
        {
            return null;
        }

        var latestVersion = versions.Max(v => v.Version);

        return latestVersion;
    }
}