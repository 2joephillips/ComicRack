using ComicRack.Data.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace ComicReader.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) => {

                //var appSettings = context.Configuration.GetSection("AppSettings").Get<AppSettings>();
                //services.AddSingleton(appSettings);

                // Register DbContext
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite("Data Source=comics.db"));

                // Register database initializer
                services.AddTransient<DatabaseInitializer>();

                // Register MainWindow
                services.AddSingleton<MainWindow>();
            })
               .ConfigureLogging(logging =>
               {
                   logging.ClearProviders();
                   logging.AddConsole();
               })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

     
        // Resolve and run the database initializer
        var databaseInitializer = _host.Services.GetRequiredService<DatabaseInitializer>();
        await databaseInitializer.InitializeDatabaseAsync();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);


    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();

        base.OnExit(e);
    }
}
