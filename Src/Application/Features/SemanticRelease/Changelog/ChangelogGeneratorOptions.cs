﻿namespace Samuel.Application.Features.SemanticRelease.Changelog;

public class ChangelogGeneratorOptions
{
    public const string SectionName = "ChangelogGenerator";
    public required string OutputFilePath { get; set; }
    public string? IssueUrlFormat { get; set; }
    public required string Title { get; set; }
    public required string FeatureSectionTitle { get; set; }
    public required string FixSectionTitle { get; set; }
}