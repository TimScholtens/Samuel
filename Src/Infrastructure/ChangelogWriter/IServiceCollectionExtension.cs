using Microsoft.Extensions.DependencyInjection;
using Samuel.Application.Features.SemanticRelease.Changelog;

namespace Samuel.Infrastructure.ChangelogWriter;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddMarkdownWriter(this IServiceCollection services)
    {
        services.AddTransient<IChangelogWriter, MarkdownWriter>();


        return services;
    }
}
