using Microsoft.Extensions.DependencyInjection;
using PrisonBlock.Core;
using PrisonBlock.Core.Storage;
using PrisonBlock.WpfApp.Services;
using PrisonBlock.WpfApp.ViewModels;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using PrisonBlock.Core.Storage.EfCore;

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
        // Valib LocalDB teostuse DI registreerimisel
        if (StorageProviderOptions.UseEfCore())
        {
            services.AddDbContextFactory<PrisonBlockDbContext>(options =>
                options.UseSqlServer(
                    @"Server=(localdb)\MSSQLLocalDB;Database=PrisonBlock_Vol5;Trusted_Connection=True;TrustServerCertificate=True"));
            services.AddSingleton<IWorldStorage, EfCoreLocalDbWorldStorage>();
        }
        else
        {
            services.AddSingleton<IWorldStorage, SqlLocalDbWorldStorage>();
        }

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
