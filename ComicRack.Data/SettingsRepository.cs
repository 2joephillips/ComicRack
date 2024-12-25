using ComicRack.Core;
using ComicRack.Core.Models;
using ComicRack.Data;

namespace ComicReader.Data;

public class SettingsRepository : ISettingsRepository
{
    private readonly ApplicationDbContext _context;

    public SettingsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public string? GetSetting(string key)
    {
        var setting = _context.Settings.FirstOrDefault(s => s.Key == key);
        return setting?.Value;
    }

    public void InsertOrUpdateSetting(ApplicationSettingKey enumKey, string value)
    {
        var key = enumKey.ToString();
        var setting = _context.Settings.FirstOrDefault(s => s.Key == key);
        if (setting != null)
        {
            setting.Value = value;
            _context.Settings.Update(setting);
        }
        else
        {
            _context.Settings.Add(new Setting
            {
                Key = key,
                Value = value
            });
        }
        _context.SaveChanges();
    }
}
