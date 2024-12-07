using ComicReader.Core;
using System.Text;
using System.Windows;
using System.Text.Json;
using System.IO;

namespace ComicReader.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ComicRack _comicRack;

    public MainWindow()
    {
        InitializeComponent();


       _comicRack = new ComicRack();
        Task _ = InitializeComicReaderAsync();
    }

    private async Task InitializeComicReaderAsync()
    {
        var storageUrl = _comicRack.STORAGE_URL;
        var comicLocation = _comicRack.COMIC_LIBRARY_LOCACTION;
        if (Path.Exists(storageUrl))
        {
            // read file into memory
            var data = await File.ReadAllTextAsync(storageUrl, Encoding.UTF8);
            // decode data into object
            var storedComics = JsonSerializer.Deserialize<List<Comic>>(data);
        }

        // Get all file paths in the root directory and its subdirectories
        string[] filePaths = Directory.GetFiles(comicLocation, "*.*", SearchOption.AllDirectories);

        // Optionally, you can just keep the file names instead of full paths
        var comics = filePaths.Select(filePath => new Comic(filePath, Path.GetFileName(filePath)));

        // Write all files to storage.json
        string json = JsonSerializer.Serialize(comics, new JsonSerializerOptions { WriteIndented = true, });
        await File.WriteAllTextAsync(storageUrl, json);
    }
}