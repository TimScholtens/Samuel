﻿using LibGit2Sharp;
using Microsoft.Extensions.DependencyInjection;
using Samuel.Application.Features.SemanticRelease.Shared.Git;

namespace Samuel.Infrastructure.Git;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddGitService(this IServiceCollection services)
    {
        // Add local repository.
        var repositoryPath = Repository.Discover(".");
        Repository? repository = null;
        if (repositoryPath != null) {
            repository = new Repository(repositoryPath, new RepositoryOptions() { });
        }

        services
            .AddSingleton(repository)
            .AddTransient<IGitService, GitService>()
            .AddTransient<IGitServiceMapper, GitServiceMapper>();

        return services;
    }
}
