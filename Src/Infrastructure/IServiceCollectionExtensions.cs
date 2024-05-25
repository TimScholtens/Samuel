using Microsoft.Extensions.DependencyInjection;
using Samuel.Infrastructure.ChangelogWriter;
using Samuel.Infrastructure.Git;
using Samuel.Infrastructure.Logger;
using Samuel.Infrastructure.Nuget;

namespace Samuel.Infrastructure;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services
            .AddLogger()
            .AddMarkdownWriter()
            .AddNugetService()
            .AddGitService();

        return services;
    }
}
