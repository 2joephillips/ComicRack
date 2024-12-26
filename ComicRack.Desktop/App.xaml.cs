﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Windows.Threading;

namespace ComicRack.Desktop;


public partial class App
{

    private static readonly IHost _host = Services.HostBuilder.Build();

    private void OnStartup(object sender, StartupEventArgs e)
    {
        _host.Start();
    }

    private async void OnExit(object sender, ExitEventArgs e)
    {
        await _host.StopAsync();

        _host.Dispose();
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // Log the exception
        var logger = _host.Services.GetRequiredService<ILogger<App>>();
        logger.LogError(e.Exception, "Unhandled exception occurred");

        // Optional: Show a user-friendly message
        MessageBox.Show("An unexpected error occurred. The application will close.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        // Prevent application crash
        e.Handled = true;
    }
}
