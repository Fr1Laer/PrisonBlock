using System.Text.Json;

namespace PrisonBlock.Core.Storage;

public sealed class JsonWorldStorage : IWorldStorage
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public async Task SaveAsync(
        WorldDto world,
        string path,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(world);

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Faili tee on kohustuslik", nameof(path));

        string fullPath = Path.GetFullPath(path);
        string? directory = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        await using FileStream stream = new(
            fullPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            4096,
            useAsync: true);

        await JsonSerializer.SerializeAsync(
            stream,
            world,
            Options,
            token);
    }

    public async Task<WorldDto> LoadAsync(
        string path,
        CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Faili tee on kohustuslik", nameof(path));

        await using FileStream stream = new(
            Path.GetFullPath(path),
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            4096,
            useAsync: true);

        try
        {
            WorldDto? world = await JsonSerializer.DeserializeAsync<WorldDto>(
                stream,
                Options,
                token);

            return world ?? throw new InvalidDataException(
                "JSON-fail on tühi või sisaldab vigaseid andmeid. Kontrolli faili sisu");
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException(
                "JSON-fail on tühi või sisaldab vigaseid andmeid. Kontrolli faili sisu",
                ex);
        }
    }
}