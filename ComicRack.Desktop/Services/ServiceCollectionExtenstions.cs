using ComicRack.Core;
using ComicRack.Data;
using ComicRack.Desktop.ViewModels.Pages;
using ComicRack.Desktop.ViewModels.Windows;
using ComicRack.Desktop.Views.Pages;
using ComicRack.Desktop.Views.Windows;
using ComicReader.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wpf.Ui;

namespace ComicRack.Desktop.Services
{
    public static class ServiceCollectionExtenstions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite($"Data Source={ApplicationSettings.DatabasePath}");
            });
            services.AddTransient<DatabaseHandler>();
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddHostedService<ApplicationHostService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<ITaskBarService, TaskBarService>();
            services.AddSingleton<SnackbarService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddTransient<IComicMetadataExtractor, ComicMetadataExtractor>();
            services.AddTransient<ISystemStorage, SystemStorage>();
            return services;
        }

        public static IServiceCollection AddUIComponents(this IServiceCollection services)
        {
            services.AddSingleton<INavigationWindow, MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddTransient<Reader>();
            services.AddSingleton<ReaderViewModel>();

            services.AddSingleton<DashboardPage>();
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<DataPage>();
            services.AddSingleton<DataViewModel>();
            services.AddSingleton<SettingsPage>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<StartUpPage>();
            services.AddSingleton<StartUpViewModel>();
            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton<ISettingsRepository, SettingsRepository>();
            return services;
        }
    }
}
