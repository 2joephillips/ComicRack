using ComicRack.Core;
using ComicRack.Data.Data;
using ComicReader.UI;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var folderName = SelectComicFolder();
                if (folderName == null) return;

                var files = await ScanFolder(folderName).ConfigureAwait(false); ;
                if (files == null) return;
                var comics = files.Select(file => new Comic(file)).ToList();

                var result =  MessageBox.Show("Found " + comics.Count + " comics. Do you want to start scanning?", "?", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK) {

                    // Process each comic asynchronously

                    var comicsCount = comics.Count() + 1;
                    foreach (var comic in comics)
                    {
                     

                        // Fetch metadata for the comic on a background thread
                        await Task.Run(() => comic.GetMetaData()).ConfigureAwait(false);

                        // Create a TreeViewItem for the comic and update the UI
                        // Create the TreeViewItem and update the UI on the Dispatcher thread
                        await Dispatcher.InvokeAsync(() =>
                        {
                            double completionRate = (comics.IndexOf(comic)+1) / (double)comicsCount ;
                            progress_bar.Value = completionRate * 100;
                            var item = new TreeViewItem { Header = comic.FileName };
                            comics_list.Items.Add(item);
                        });
                    }
                }
                else { }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Initialization failed: {ex.Message}");
                Close();
                return;
            }

         


        }

        private void SaveFilesToDB(List<Comic> files)
        {
            _dbContext.Comics.AddRangeAsync(files);
            _dbContext.SaveChanges();
        }

        private Task<List<string>>? ScanFolder(string folderName)
        {
          return Task.Run(() =>
            {
                var supportedExtensions = new List<string>() { ".jpg", ".png", ".pdf", ".cbz", ".cbr" };

                // Get all file paths in the root directory and its subdirectories
                var filePaths = Directory.GetFiles(folderName, "*.*", SearchOption.AllDirectories)
                    .Where(file => supportedExtensions.Contains(System.IO.Path.GetExtension(file).ToLower()))
                    .ToList();

                return filePaths;
            });
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}