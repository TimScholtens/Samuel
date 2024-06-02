using Samuel.Domain;

namespace Samuel.Application.Features.SemanticRelease.Shared.Git;

public interface IGitService
{
    public List<Commit> GetCommitsAfter(string commitSha);
    public List<Commit> GetCommits();
    public Release? GetLatestRelease();
    public List<Release> GetAllReleases();
    public void Tag(string newVersionName);
    public void CommitAll(string commitMessage);
    public void StageFile(string fileName);
    public void PushTag(string tagName);
    public void PushCommit();
}
