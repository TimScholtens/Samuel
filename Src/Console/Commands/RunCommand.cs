using Samuel.Application.Features.SemanticRelease;
using Serilog.Core;
using Serilog.Events;
using Spectre.Console.Cli;

namespace Samuel.CLI.Commands;

public class RunCommand : Command<RunCommand.Settings>
{
    private readonly ISemanticReleaser _semanticReleaser;
    private LoggingLevelSwitch _loggingLevel;

    public class Settings : CommandSettings
    {
        [CommandOption("--dry-run")]
        public bool IsDryRun { get; set; } = false;

        [CommandOption("--debug")]
        public bool Debug { get; set; } = false;
    }

    public RunCommand(ISemanticReleaser semanticReleaser, LoggingLevelSwitch loggingLevel)
    {
        _semanticReleaser = semanticReleaser;
        _loggingLevel = loggingLevel;
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        if (settings.Debug)
        {
            _loggingLevel.MinimumLevel = LogEventLevel.Debug;
        }

        return _semanticReleaser.Run(new SemanticReleaserSettings()
        {
            IsDryRun = settings.IsDryRun
        });
    }
}
