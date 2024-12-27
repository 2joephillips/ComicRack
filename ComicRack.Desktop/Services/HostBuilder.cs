using ComicRack.Core;
using ComicRack.Data;
using ComicRack.Desktop.ViewModels.Pages;
using ComicRack.Desktop.ViewModels.Windows;
using ComicRack.Desktop.Views.Pages;
using ComicRack.Desktop.Views.Windows;
using ComicReader.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;
using Wpf.Ui;

namespace ComicRack.Desktop.Services;

public static class HostBuilder
{
    // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public static IHost Build()
    {
        return Host
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration(c => { c.SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)); })
        .ConfigureServices((context, services) =>
        {
            // Register DbContext
            services
                .AddDatabase()
                .AddServices()
                .AddUIComponents()
                .AddRepositories();
        })
        .Build();
    }
}
