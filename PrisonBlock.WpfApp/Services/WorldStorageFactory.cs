using PrisonBlock.Core.Storage;

namespace PrisonBlock.WpfApp.Services;

public sealed class WorldStorageFactory
{
    private readonly JsonWorldStorage _jsonStorage;
    private readonly XmlWorldStorage _xmlStorage;
    private readonly IWorldStorage _localDbStorage;

    public WorldStorageFactory(
        JsonWorldStorage jsonStorage,
        XmlWorldStorage xmlStorage,
        IWorldStorage localDbStorage)
    {
        _jsonStorage = jsonStorage;
        _xmlStorage = xmlStorage;
        _localDbStorage = localDbStorage;
    }

    public IWorldStorage GetStorage(string format)
    {
        return format.ToUpperInvariant() switch
        {
            "JSON" => _jsonStorage,
            "XML" => _xmlStorage,
            "LOCALDB" => _localDbStorage,
            _ => throw new ArgumentException(
                "Tundmatu salvestusvorming",
                nameof(format))
        };
    }
}
