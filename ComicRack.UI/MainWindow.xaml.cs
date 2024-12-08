using System.Windows;
using System.IO;
using Microsoft.Win32;
using ComicRack.Core;
using ComicRack.Data.Data;

namespace ComicReader.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private ComicBin _comicRack;

    public MainWindow()
    {
        InitializeComponent();


        _comicRack = new ComicBin();
    
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var folderName = SelectComicFolder();
        if (folderName == null) return;

       var files = ScanFolder(folderName);
        if(files == null) return;
       var comics = files.Select(file => new Comic(file)).ToList();
        SaveFilesToDB(comics);
    }

    private void SaveFilesToDB(List<Comic> files)
    {
        using (var context = new ApplicationDbContext())
        {
            context.Comics.AddRangeAsync(files);
            context.SaveChanges();
        }
    }

    private List<string>? ScanFolder(string folderName)
    {
        var comicLocation = _comicRack.COMIC_LIBRARY_LOCACTION;
        var supportedExtensions = new List<string>() { ".jpg", ".png", ".pdf", ".cbz", ".cbr" };

        // Get all file paths in the root directory and its subdirectories
        var filePaths = Directory.GetFiles(comicLocation, "*.*", SearchOption.AllDirectories)
            .Where(file => supportedExtensions.Contains(Path.GetExtension(file).ToLower()))
            .ToList();

        return filePaths;
    }

    private string? SelectComicFolder()
    {
        var folderDialog = new OpenFolderDialog();
        if (folderDialog.ShowDialog() == true)
        {
            return folderDialog.FolderName;
        }

        return null;
    }
}