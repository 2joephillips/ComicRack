using ComicRack.Core;
using ComicRack.Desktop.Views.Pages;
using ComicRack.Desktop.Views.Windows;
using ComicReader.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows.Navigation;
using Wpf.Ui;
using Wpf.Ui.Appearance;

namespace ComicRack.Desktop.Services;

/// <summary>
/// Managed host of the application.
/// </summary>
public class ApplicationHostService : IHostedService
{
  private readonly IServiceProvider _serviceProvider;
  private readonly DatabaseHandler _databaseHandler;
  private readonly INavigationWindow _navigationWindow;

  public ApplicationHostService(IServiceProvider serviceProvider, DatabaseHandler databaseHandler, INavigationWindow navigationWindow)
  {
    _serviceProvider = serviceProvider;
    _databaseHandler = databaseHandler;
    _navigationWindow = navigationWindow;
  }

  /// <summary>
  /// Triggered when the application host is ready to start the service.
  /// </summary>
  /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
  public async Task StartAsync(CancellationToken cancellationToken)
  {
    await HandleActivationAsync();
  }

  public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

  /// <summary>
  /// Creates main window during activation.
  /// </summary>
  private async Task HandleActivationAsync()
  {
    var anyMainWindows = Application.Current.Windows.OfType<MainWindow>().Any();

    ApplicationSettings.EnsureAppDataFolderExists();

    _databaseHandler.EnsureDatabaseInitialized();

    var settings = await _databaseHandler.InitializeSettings();
    ApplicationSettings.Apply(settings);

    ApplicationThemeManager.Apply(ApplicationSettings.CurrentTheme);

    //var _navigationWindow = (
    //        _serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow
    //    )!;
    _navigationWindow!.ShowWindow();

    if (ApplicationSettings.IsSetUpComplete)
      _navigationWindow.Navigate(typeof(DashboardPage));
    else
      _navigationWindow.Navigate(typeof(StartUpPage));
    await Task.CompletedTask;
  }
}
