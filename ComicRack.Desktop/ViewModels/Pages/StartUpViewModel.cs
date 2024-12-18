
using ComicRack.Core.Models;
using ComicRack.Core;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace ComicRack.Desktop.ViewModels.Pages
{
    public partial class StartUpViewModel : ObservableObject
    {
        private readonly IComicMetadataExtractor _extractor;

        public StartUpViewModel(IComicMetadataExtractor extractor)
        {
            _extractor = extractor;
        }

        [ObservableProperty]
        private double _progressValue = 0;

        [ObservableProperty]
        private string _selectedFolderText = "Folder Not Selected";

        [ObservableProperty]
        private string _currentImagePath = "";

        [ObservableProperty]
        private bool _loading = false;

        [ObservableProperty]
        private ObservableCollection<Comic> _comicsCollection = new ObservableCollection<Comic>();

        [ObservableProperty]
        private int _selectedComic = 1;

        [RelayCommand]
        private async Task PickFolderAsync()
        {
            try
            {
                Loading = true;
                var folderName = SelectComicFolder();
                if (folderName == null) {
                    SelectedFolderText = "Folder Not Selected";
                    return;
                }

                SelectedFolderText = folderName;
                return;

 
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        [RelayCommand]
        private async Task ScanFolderAsync()
        {
            try
            {
                var files = await FolderHandler.ScanFolder(SelectedFolderText).ConfigureAwait(false); ;
                if (files == null || !files.Any()) return;


                var comics = files.Select(file => new Comic(file, _extractor)).ToList();

                var result = MessageBox.Show("Found " + comics.Count + " comics. Do you want to start scanning?", "Comics Found", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    foreach (var comic in comics)
                    {
                        // Fetch metadata for the comic on a background thread
                        await Task.Run(() => comic.LoadMetaData()).ConfigureAwait(false);
                        CurrentImagePath = comic.CoverImagePaths.HighResPath;

                        App.Current.Dispatcher.Invoke((Action)delegate
                        {
                            ComicsCollection.Add(comic);
                        });
                 
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Initialization failed: {ex.Message}");
                //Close();}
            }
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