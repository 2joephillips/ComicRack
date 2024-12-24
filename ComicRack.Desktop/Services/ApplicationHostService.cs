using ComicRack.Core;
using ComicRack.Desktop.Views.Pages;
using ComicRack.Desktop.Views.Windows;
using ComicReader.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;
using Wpf.Ui.Appearance;

namespace ComicRack.Desktop.Services
{
    /// <summary>
    /// Managed host of the application.
    /// </summary>
    public class ApplicationHostService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        private INavigationWindow _navigationWindow;

        public ApplicationHostService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await HandleActivationAsync();
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Creates main window during activation.
        /// </summary>
        private async Task HandleActivationAsync()
        {
            if (!Application.Current.Windows.OfType<MainWindow>().Any())
            {
                ApplicationSettings.EnsureAppDataFolderExists();

                var databaseInitializer = _serviceProvider.GetRequiredService<DatabaseHandler>();
                databaseInitializer.EnsureDatabaseInitialized();

                var settings = await databaseInitializer.InitializeSettings();
                ApplicationSettings.Apply(settings);

                ApplicationThemeManager.Apply(ApplicationSettings.CurrentTheme);

                _navigationWindow = (
                        _serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow
                    )!;
                _navigationWindow!.ShowWindow();

                if (ApplicationSettings.IsSetUpComplete)
                    _navigationWindow.Navigate(typeof(DashboardPage));
                else
                    _navigationWindow.Navigate(typeof(StartUpPage));
            }

            await Task.CompletedTask;
        }
    }
}
