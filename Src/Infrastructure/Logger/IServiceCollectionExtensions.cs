using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Samuel.Infrastructure.Logger;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddLogger(this IServiceCollection services)
    {
        var levelSwitch = new LoggingLevelSwitch(LogEventLevel.Information);
        var logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(levelSwitch)
            .WriteTo.Console()
            .CreateLogger();

        services
            .AddLogging(configure => configure.AddSerilog(logger))
            .AddSingleton(levelSwitch);

        return services;
    }

}
