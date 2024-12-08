using ComicRack.Data.Data;

namespace ComicReader.Data
{
    public class DBSeeder
    {
        public static async Task SeedInitialSettingsAsync(ApplicationDbContext context)
        {
            var anySettings = context.Settings.Any();
            if (anySettings) return;
            context.Settings.AddRange(
                new ComicRack.Core.Setting { Key = "Theme", Value = "Lite" },
                new ComicRack.Core.Setting { Key = "SetUpComplete", Value = "false" }
                );
             await context.SaveChangesAsync();
        }
    }
}