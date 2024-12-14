using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ComicReader.UI;
using ComicRack.Data.Data;
using ComicRack.Core;
using System.IO;

namespace ComicRack.UI
{
    class Program
    {
        private static IHost _host;


        [STAThread]
        static void Main(string[] args)
        {
            // Start the WPF application on a dedicated STA thread
            Thread uiThread = new Thread(WpfApp)
            {
                IsBackground = false, // Keep the thread alive until the application exits
            };
            uiThread.SetApartmentState(ApartmentState.STA);
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

        private static void WpfApp()
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
                             services.AddSingleton<Bootstrapper>();
                             services.AddSingleton<IComicMetadataExtractor, ComicMetadataExtractor>();
                             services.AddSingleton<ISystemStorage, SystemStorage>();

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
            var bootstrap = _host.Services.GetRequiredService<Bootstrapper>();
            bootstrap.Run();
        }
    }
}
