using ComicRack.Core;

namespace ComicReader.Data
{
    public interface ISettingsRepository
    {
        string? GetSetting(string key);
        void InsertOrUpdateSetting(ApplicationSettingKey key, string value);
    }
}