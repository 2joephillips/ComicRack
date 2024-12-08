using ComicRack.Data.Data;
using ComicReader.Data;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ComicReader.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        using(var context = new ApplicationDbContext())
        {
            try
            {
                context.Database.Migrate();
                Console.WriteLine("Database migrated and up-to-date.");
                DBSeeder.SeedInitialSettingsAsync(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error During migration: {ex.Message}");
                throw;
            }
        }
    }
}
