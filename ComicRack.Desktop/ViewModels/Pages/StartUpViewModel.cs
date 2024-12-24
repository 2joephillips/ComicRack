
using ComicRack.Core.Models;
using ComicRack.Core;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using ComicRack.Desktop.Views.Windows;
using System.Windows.Input;
using Wpf.Ui;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace ComicRack.Desktop.ViewModels.Pages
{
    public partial class StartUpViewModel : ObservableObject
    {
        const string FOLDER_NOT_SELECTED = "Folder Not Selected";

        private readonly IComicMetadataExtractor _extractor;
        private readonly IServiceProvider _serviceProvider;

        public IRelayCommand ScanCommand { get; }
        public ICommand ShowInfo { get; }

        public StartUpViewModel(IComicMetadataExtractor extractor, IServiceProvider serviceProvider)
        {
            _extractor = extractor;
            _serviceProvider = serviceProvider;
            ScanCommand = new RelayCommand(ScanFolderAsync, CanScan);
            ShowInfo = new RelayCommand<Comic>(ShowComicInfo);
        }


        [ObservableProperty]
        private string _selectedFolderText = FOLDER_NOT_SELECTED;

        [ObservableProperty]
        private string _scanningProgress = string.Empty;

        [ObservableProperty]
        private string _currentImagePath = "";

        [ObservableProperty]
        private bool _loading = false;

        [ObservableProperty]
        private ObservableCollection<Comic> _comicsCollection = new ObservableCollection<Comic>();

        [ObservableProperty]
        private int _selectedComic = 1;


        private void ShowComicInfo(Comic selectedComic)
        {
            if (selectedComic != null)
            {
                // Logic to display or process the selected comic
                MessageBox.Show($"Title: {selectedComic.Title}, Publisher: {selectedComic.Publisher}");
            }
        }

        [RelayCommand]
        private async Task PickFolderAsync()
        {
            try
            {
                var tempSelectedFolderText = SelectComicFolder;
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

        private async void ScanFolderAsync()
        {
            try
            {
                var files = await FolderHandler.ScanFolder(SelectedFolderText).ConfigureAwait(false); ;
                if (files == null || !files.Any()) return;


                var comics = files.Select(file => new Comic(file, _extractor)).ToList();

                var result = MessageBox.Show("Found " + comics.Count + " comics. Do you want to start scanning?", "Comics Found", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    var index = 0;
                    ScanningProgress = ProgressText(comics.Count, index);
                    foreach (var comic in comics)
                    {
                        ScanningProgress = ProgressText(comics.Count, index++);
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

        private string ProgressText(int count, int index)
        {
            return $"Progress: {index}/{count}";
        }

        [RelayCommand]
        private async Task OpenSelectedComicAsync()
        {
            var selectedComicId = SelectedComic;
            var selectedComic = this.ComicsCollection[selectedComicId];

            if (selectedComic == null) return;

            //var reader  = Application.Current.ServiceProvider.GetRequiredService<Reader>();
            var reader = _serviceProvider.GetRequiredService<Reader>();
            reader.SetUp(selectedComic);
            reader.Show();
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

        private bool CanScan()
        {
            return SelectedFolderText != FOLDER_NOT_SELECTED;
        }

        partial void OnSelectedFolderTextChanged(string value)
        {
            ScanCommand.NotifyCanExecuteChanged();
        }
    }
}