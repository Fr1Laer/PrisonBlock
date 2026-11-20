
using Microsoft.Win32;

namespace PrisonBlock.WpfApp.Services;

public interface IStorageDialogService
{
    string? GetSavePath(string format);
    string? GetLoadPath(string format);
}

public sealed class StorageDialogService : IStorageDialogService
{
    public string? GetSavePath(string format)
    {
        SaveFileDialog dialog = new()
        {
            Title = "Salvesta vangla maailm",
            FileName = "PrisonBlock",
            AddExtension = true,
            OverwritePrompt = true
        };

        switch (format.ToUpperInvariant())
        {
            case "JSON":
                dialog.DefaultExt = ".json";
                dialog.Filter = "JSON fail (*.json)|*.json";
                break;

            case "XML":
                dialog.DefaultExt = ".xml";
                dialog.Filter = "XML fail (*.xml)|*.xml";
                break;

            case "LOCALDB":
                return "LocalDB";

            default:
                throw new ArgumentException("Tundmatu salvestusvorming", nameof(format));
        }

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public string? GetLoadPath(string format)
    {
        if (format.Equals("LOCALDB", StringComparison.OrdinalIgnoreCase))
            return "LocalDB";

        OpenFileDialog dialog = new()
        {
            Title = "Laadi vangla maailm",
            CheckFileExists = true,
            Multiselect = false
        };

        switch (format.ToUpperInvariant())
        {
            case "JSON":
                dialog.DefaultExt = ".json";
                dialog.Filter = "JSON fail (*.json)|*.json";
                break;

            case "XML":
                dialog.DefaultExt = ".xml";
                dialog.Filter = "XML fail (*.xml)|*.xml";
                break;

            default:
                throw new ArgumentException("Tundmatu salvestusvorming", nameof(format));
        }

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
