﻿using ComicRack.Core.Models;
using System.IO;
using Wpf.Ui.Appearance;

namespace ComicRack.Core;

public enum ApplicationSettingKey
{
    SetupComplete,
    ThemeColor,
    RootFolder
}

public static class ApplicationSettings 
{
    public static bool IsSetUpComplete { get; private set; } = false;
    public static ApplicationTheme CurrentTheme { get; private set; } = ApplicationTheme.Unknown;
    public static string? RootFolder { get; private set; }
    public static string AppDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ComicRack");
    public static string DatabasePath => Path.Combine(AppDataPath, "comics.db");

    /// <summary>
    /// Ensures that the application data folder exists.
    /// </summary>
    public static void EnsureAppDataFolderExists()
    {
        if (!Directory.Exists(AppDataPath))
        {
            Directory.CreateDirectory(AppDataPath);
        }
    }

    /// <summary>
    /// Applies settings from a list of key-value pairs.
    /// </summary>
    /// <param name="settings">A list of settings to apply.</param>
    public static void Apply(List<Setting> settings)
    {
        if (settings == null || settings.Count == 0)
        {
            Console.WriteLine("No settings provided to apply.");
            return;
        }

        try
        {
            var isSetupComplete = settings.FirstOrDefault(s => s.Key == ApplicationSettingKey.SetupComplete.ToString())?.Value;
            IsSetUpComplete = isSetupComplete != null && bool.TryParse(isSetupComplete, out bool setupComplete) && setupComplete;

            var currentTheme = settings.FirstOrDefault(s => s.Key == ApplicationSettingKey.ThemeColor.ToString())?.Value;
            CurrentTheme = Enum.TryParse(typeof(ApplicationTheme), currentTheme, true, out var theme)
                ? (ApplicationTheme)theme
                : ApplicationTheme.Unknown;

            var rootFolder = settings.FirstOrDefault( s => s.Key == ApplicationSettingKey.RootFolder.ToString())?.Value;
            RootFolder = rootFolder;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error applying settings: {ex.Message}");
            // Optional: Log error details to a log file
        }
    }
}

