using ComicRack.Core;
using ComicRack.Data.Data;
using ComicReader.UI;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace ComicRack.UI
{
    /// <summary>
    /// Interaction logic for StartUp.xaml
    /// </summary>
    public partial class StartUp : Window
    {
        private ApplicationDbContext _dbContext;
        private MainWindow _mainWindow;
        private ComicBin _comicRack;

        public StartUp(ApplicationDbContext dbContext, MainWindow mainWindow)
        {
            InitializeComponent();
            _dbContext = dbContext;
            _mainWindow = mainWindow;
            _comicRack = new ComicBin();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var folderName = SelectComicFolder();
                if (folderName == null) return;

                var files = ScanFolder(folderName);
                if (files == null) return;
                var comics = files.Select(file => new Comic(file)).ToList();
                SaveFilesToDB(comics);
                MessageBox.Show("Initialization complete!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Initialization failed: {ex.Message}");
                Close();
                return;
            }

            // Open the MainWindow
            _mainWindow.Show();

            // Close the StartupWindow
            Close();


        }

        private void SaveFilesToDB(List<Comic> files)
        {
            _dbContext.Comics.AddRangeAsync(files);
            _dbContext.SaveChanges();
        }

        private List<string>? ScanFolder(string folderName)
        {
            var comicLocation = _comicRack.COMIC_LIBRARY_LOCACTION;
            var supportedExtensions = new List<string>() { ".jpg", ".png", ".pdf", ".cbz", ".cbr" };

            // Get all file paths in the root directory and its subdirectories
            var filePaths = Directory.GetFiles(comicLocation, "*.*", SearchOption.AllDirectories)
                .Where(file => supportedExtensions.Contains(System.IO.Path.GetExtension(file).ToLower()))
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
}