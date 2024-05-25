using Microsoft.Extensions.DependencyInjection;
using Samuel.Application.Features.SemanticRelease.ApplicationVersionChecker;

namespace Samuel.Infrastructure.Nuget;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddNugetService(this IServiceCollection services)
    {
        services
            .AddTransient<INugetService, NugetService>();

        return services;
    }
}
