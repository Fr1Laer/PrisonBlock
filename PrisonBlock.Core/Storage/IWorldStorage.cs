namespace PrisonBlock.Core.Storage;

public interface IWorldStorage
{
    Task SaveAsync(
        WorldDto world,
        string path,
        CancellationToken token = default);

    Task<WorldDto> LoadAsync(
        string path,
        CancellationToken token = default);
}