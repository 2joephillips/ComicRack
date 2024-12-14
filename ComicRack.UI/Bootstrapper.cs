using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ComicReader.UI;

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
            // Start the host
            await _host.StartAsync();

            var databaseInitializer = _serviceProvider.GetRequiredService<DatabaseInitializer>();
            var wasInitialized = await databaseInitializer.InitializeDatabaseAsync();

            if (wasInitialized.SetupCompleted)
            {
                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
               app.Run(mainWindow);
            }
            else
            {
                var startUp = _serviceProvider.GetRequiredService<StartUp>();
                app.Run(startUp);
            }

            await _host.StopAsync();
        }
    }
}
