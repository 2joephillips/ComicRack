using ComicRack.Data.Data;
using ComicReader.Data;
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

        public async Task<bool> InitializeDatabaseAsync()
        {
            try
            {
                // Apply migrations
                await _context.Database.MigrateAsync();
                Console.WriteLine("Database migrated and up-to-date.");

                // Seed initial settings
                return await DBSeeder.SeedInitialSettingsAsync(_context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during migration: {ex.Message}");
                throw;
            }
        }
    }
}