using System;
using System.Threading;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ComicReader.UI;
using ComicRack.Data.Data;

namespace ComicRack.UI
{
    class Program
    {
        private static IHost _host;


        [STAThread]
        static void Main(string[] args)
        {
            // Start the WPF application on a dedicated STA thread
            Thread uiThread = new Thread(StartWpfApp)
            {
                IsBackground = false, // Keep the thread alive until the application exits
                ApartmentState = ApartmentState.STA // Ensure STA model
            };

            uiThread.Start();


            // Separate high-priority thread for background tasks
            Thread highPriorityThread = new Thread(PerformHighPriorityTasks)
            {
                IsBackground = true, // Allows the process to exit when the main thread exits
                Priority = ThreadPriority.Highest
            };

            highPriorityThread.Start();
        }

        private static void PerformHighPriorityTasks(object? obj)
        {
            while (true)
            {
                Console.WriteLine("Performing high-priority tasks...");
                Thread.Sleep(500); // Simulate work
            }
        }

        private static void StartWpfApp()
        {
            _host = Host.CreateDefaultBuilder()
                         .ConfigureServices((context, services) =>
                         {
                             // Add application settings
                             // var appSettings = context.Configuration.GetSection("AppSettings").Get<AppSettings>();
                             // services.AddSingleton(appSettings);

                             // Register DbContext
                             services.AddDbContext<ApplicationDbContext>(options =>
                                 options.UseSqlite("Data Source=comics.db"));

                             // Register database initializer
                             services.AddTransient<DatabaseInitializer>();
                             services.AddSingleton<StartUpApp>();

                             // Register WPF components
                             services.AddSingleton<MainWindow>();
                             services.AddSingleton<StartUp>();
                         })
                         .ConfigureLogging(logging =>
                         {
                             logging.ClearProviders();
                             logging.AddConsole();
                         })
                         .Build();

            // Start the WPF application
            var startup = _host.Services.GetRequiredService<StartUpApp>();
            startup.Run();
        }
    }

    public class StartUpApp
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHost _host;

        public StartUpApp(IServiceProvider serviceProvider, IHost host)
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
