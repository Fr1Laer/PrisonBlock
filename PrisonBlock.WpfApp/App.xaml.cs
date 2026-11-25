using Microsoft.Extensions.DependencyInjection;
using PrisonBlock.Core;
using PrisonBlock.Core.Storage;
using PrisonBlock.WpfApp.Services;
using PrisonBlock.WpfApp.ViewModels;
using System.Windows;

namespace PrisonBlock.WpfApp;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        ServiceCollection services = new();

        services.AddSingleton<PrisonService>();

        services.AddSingleton<JsonWorldStorage>();
        services.AddSingleton<XmlWorldStorage>();
        services.AddSingleton<SqlLocalDbWorldStorage>();

        services.AddSingleton<WorldStorageFactory>();
        services.AddSingleton<IStorageDialogService, StorageDialogService>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();

        _serviceProvider = services.BuildServiceProvider();

        MainWindow window =
            _serviceProvider.GetRequiredService<MainWindow>();

        window.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}