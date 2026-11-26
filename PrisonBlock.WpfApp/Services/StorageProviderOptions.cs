namespace PrisonBlock.WpfApp.Services;

// Valib käivitamisel kasutatava andmebaasiteostuse
public static class StorageProviderOptions
{
    public static bool UseEfCore()
    {
        string? provider = Environment.GetEnvironmentVariable("PRISONBLOCK_DATABASE_PROVIDER");
        return !string.Equals(provider, "ADO", StringComparison.OrdinalIgnoreCase);
    }
}
