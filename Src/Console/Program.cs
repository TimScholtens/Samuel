using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Samuel.Application;
using Samuel.Application.Features.SemanticRelease.Changelog;
using Samuel.Application.Features.SemanticRelease.Publisher;
using Samuel.CLI.Commands;
using Samuel.CLI.Configuration;
using Samuel.CLI.DI;
using Samuel.Infrastructure;
using Samuel.Infrastructure.Git;
using Samuel.Infrastructure.Nuget;
using Spectre.Console.Cli;
using System.Reflection;

namespace Samuel.CLI;

public class Program
{
    public static int Main(string[] args)
    {
        // Config
        IConfiguration config = new ConfigurationBuilder()
            .AddDefaultConfiguration()
            .AddOptionalConfiguration()
            .AddUserSecrets(Assembly.GetExecutingAssembly())
            .AddEnvironmentVariables()
            .Build();

        // Services
        var registrations = new ServiceCollection()
            .AddApplication()
            .AddInfrastructure()
            .Configure<ChangelogGeneratorOptions>(config.GetSection(ChangelogGeneratorOptions.SectionName))
            .Configure<PublisherOptions>(config.GetSection(PublisherOptions.SectionName))
            .Configure<GitOptions>(config.GetSection(GitOptions.SectionName))
            .Configure<NugetOptions>(config.GetSection(NugetOptions.SectionName));

        var registrar = new TypeRegistrar(registrations);
        var app = new CommandApp(registrar);

        // Setup commands.
        app.Configure(config =>
        {
            config.Settings.CaseSensitivity = CaseSensitivity.None;
            config.AddCommand<RunCommand>("Run");
        });

        return app.Run(args);
    }
}


