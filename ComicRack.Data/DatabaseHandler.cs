using ComicRack.Core;
using ComicRack.Core.Models;
using ComicRack.Data.Data;
using Microsoft.EntityFrameworkCore;
using Wpf.Ui.Appearance;

namespace ComicReader.UI
{
    public  class DatabaseHandler
    {
        private readonly ApplicationDbContext _context;

        public DatabaseHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async void EnsureDatabaseInitialized()
        {
            try
            {
                // Apply migrations
                await _context.Database.MigrateAsync();
                Console.WriteLine("Database migrated and up-to-date.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during migration: {ex.Message}");
                throw;
            }
        }


        public async Task<List<Setting>> InitializeSettings()
        {
            var anySettings = _context.Settings.Any();
            if (anySettings)
            {
                return _context.Settings.ToList();
            };
            _context.Settings.AddRange(
                new Setting { Key = ApplicationSettings.SETUPCOMPLETE, Value = "false" },
                new Setting { Key = ApplicationSettings.THEMECOLOR, Value = ApplicationTheme.Dark.ToString() }
                );
            await _context.SaveChangesAsync();
            return _context.Settings.ToList() ;
        }
    }
}