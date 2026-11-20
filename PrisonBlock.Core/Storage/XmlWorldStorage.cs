using System.Xml;
using System.Xml.Serialization;

namespace PrisonBlock.Core.Storage;

public sealed class XmlWorldStorage : IWorldStorage
{
    private static readonly XmlSerializer Serializer =
        new(typeof(WorldDto));

    public async Task SaveAsync(
        WorldDto world,
        string path,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(world);

        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Faili tee on kohustuslik", nameof(path));

        token.ThrowIfCancellationRequested();

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

        using XmlWriter writer = XmlWriter.Create(
            stream,
            new XmlWriterSettings
            {
                Async = true,
                Indent = true,
                Encoding = System.Text.Encoding.UTF8
            });

        token.ThrowIfCancellationRequested();

        Serializer.Serialize(writer, world);

        await writer.FlushAsync();
    }

    public async Task<WorldDto> LoadAsync(
        string path,
        CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Faili tee on kohustuslik", nameof(path));

        token.ThrowIfCancellationRequested();

        await using FileStream stream = new(
            Path.GetFullPath(path),
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            4096,
            useAsync: true);

        try
        {
            using XmlReader reader = XmlReader.Create(stream);

            token.ThrowIfCancellationRequested();

            return Serializer.Deserialize(reader) as WorldDto
                ?? throw new InvalidDataException(
                    "XML-fail on tühi või sisaldab vigaseid andmeid. Kontrolli faili sisu");
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidDataException(
                "XML-fail on tühi või sisaldab vigaseid andmeid. Kontrolli faili sisu",
                ex);
        }
        catch (XmlException ex)
        {
            throw new InvalidDataException(
                "XML-fail on tühi või sisaldab vigaseid andmeid. Kontrolli faili sisu",
                ex);
        }
    }
}