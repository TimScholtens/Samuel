namespace Samuel.Infrastructure.Nuget;

public class NugetOptions
{
    public const string SectionName = "Nuget";
    public required string Source { get; set; }
}