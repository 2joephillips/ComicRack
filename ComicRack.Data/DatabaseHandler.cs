using ComicRack.Core;
using ComicRack.Core.Models;
using ComicRack.Data.Data;
using Microsoft.EntityFrameworkCore;

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


        public async Task<bool> InitializeSettings()
        {
            var anySettings = _context.Settings.Any();
            if (anySettings)
            {
                var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Key == "SetUpComplete");
                var setupComplete = setting != null && setting.Value == "true";
                return (setupComplete);
            };
            _context.Settings.AddRange(
                new Setting { Key = "SetUpComplete", Value = "false" }
                );
            await _context.SaveChangesAsync();
            return false;
        }
    }
}