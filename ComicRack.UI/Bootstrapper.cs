using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ComicReader.UI;
using ComicRack.Core;

namespace ComicRack.UI
{
    public class Bootstrapper
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHost _host;

        public Bootstrapper(IServiceProvider serviceProvider, IHost host)
        {
            _serviceProvider = serviceProvider;
            _host = host;
        }

        public async void Run()
        {
            var app = new Application();
            await _host.StartAsync();

            var setupCompleted = await InitializeApplication();


            if (setupCompleted)
            {
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                app.Run(mainWindow);
            }
            else {
                var startUp = _serviceProvider.GetRequiredService<StartUp>();
                app.Run(startUp);
            }
            await _host.StopAsync();
        }

        private async Task<bool> InitializeApplication()
        {
            ApplicationSettings.EnsureAppDataFolderExists();

            var databaseInitializer = _serviceProvider.GetRequiredService<DatabaseHandler>();
            databaseInitializer.EnsureDatabaseInitialized();

            return await databaseInitializer.InitializeSettings();
        }
    }
}

