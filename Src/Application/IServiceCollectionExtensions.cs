using Microsoft.Extensions.DependencyInjection;
using Samuel.Application.Features.SemanticRelease;

namespace Samuel.Application;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<ISemanticReleaser, SemanticReleaser>();

        return services;
    }
}
