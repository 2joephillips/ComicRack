using ComicRack.Core;
using ComicRack.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace ComicReader.UI
{
    public  class DatabaseInitializer
    {
        private readonly ApplicationDbContext _context;

        public DatabaseInitializer(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Initilized, bool SetupCompleted)> InitializeDatabaseAsync()
        {
            try
            {
                // Apply migrations
                await _context.Database.MigrateAsync();
                Console.WriteLine("Database migrated and up-to-date.");

                // Seed initial settings
                return await SeedInitialSettingsAsync(_context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during migration: {ex.Message}");
                throw;
            }
        }


        private static async Task<(bool Initilized, bool SetupCompleted)> SeedInitialSettingsAsync(ApplicationDbContext context)
        {
            var anySettings = context.Settings.Any();
            if (anySettings)
            {
                var setting = await context.Settings.FirstOrDefaultAsync(s => s.Key == "SetUpComplete");
                var setupComplete = setting != null && setting.Value == "true";
                return (false, setupComplete);
            };
            context.Settings.AddRange(
                new Setting { Key = "Theme", Value = "Lite" },
                new Setting { Key = "SetUpComplete", Value = "false" }
                );
            await context.SaveChangesAsync();
            return (true, false);
        }
    }
}