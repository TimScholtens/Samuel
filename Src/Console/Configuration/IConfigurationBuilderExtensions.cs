using Microsoft.Extensions.Configuration;

namespace Samuel.CLI.Configuration;

public static class IConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddDefaultConfiguration(this IConfigurationBuilder builder)
    {
        // Add file from application's physical location.
        builder.AddJsonStream(File.Open($"{AppDomain.CurrentDomain.BaseDirectory}/Configuration.default.json", FileMode.Open));

        return builder;
    }

    public static IConfigurationBuilder AddOptionalConfiguration(this IConfigurationBuilder builder)
    {
        // Add file from current work directory.
        var optionalConfigurationPath = $"{Environment.CurrentDirectory}/Configuration.json";
        if (File.Exists(optionalConfigurationPath))
        {
            builder.AddJsonStream(File.Open(optionalConfigurationPath, FileMode.Open));
        }

        return builder;
    }
}
