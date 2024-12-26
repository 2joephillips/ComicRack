
using ComicRack.Core.Models;
using ComicRack.Core;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using ComicRack.Desktop.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using ComicReader.Data;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace ComicRack.Desktop.ViewModels.Pages
{
  public partial class StartUpViewModel : ObservableObject
  {
    private const string FOLDER_NOT_SELECTED = "Folder Not Selected";

    private readonly IComicMetadataExtractor _extractor;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISettingsRepository _settingsRepo;
    private readonly ISnackbarService _snackbarService;
    [ObservableProperty]
    private string _selectedFolderText = string.Empty;

    [ObservableProperty]
    private string _scanningProgress = string.Empty;

    [ObservableProperty]
    private string _currentImagePath = ImageHandler.DefaultHighResImageLocation;

    [ObservableProperty]
    private bool _loading;

    [ObservableProperty]
    private ObservableCollection<Comic> _comicsCollection = [];

    [ObservableProperty]
    private int _selectedComic = 1;

    public IRelayCommand ScanCommand { get; }
    public IRelayCommand SaveRootFolderCommand { get; }
    public IRelayCommand ShowInfoCommand { get; }
    public IRelayCommand PickFolderCommand { get; }
    public IRelayCommand OpenSelectedComicCommand { get; }

    public StartUpViewModel(IComicMetadataExtractor extractor, IServiceProvider serviceProvider, ISettingsRepository settingsRepo, ISnackbarService snackbarService)
    {
      _extractor = extractor;
      _serviceProvider = serviceProvider;
      _settingsRepo = settingsRepo;
      _snackbarService = snackbarService;

      ScanCommand = new RelayCommand(ScanFolderAsync, CanScanOrSave);
      SaveRootFolderCommand = new RelayCommand(SaveRootFolderAsync, CanScanOrSave);
      ShowInfoCommand = new RelayCommand<Comic>(ShowComicInfo);
      PickFolderCommand = new RelayCommand(PickFolderAsync);
      OpenSelectedComicCommand = new RelayCommand(OpenSelectedComicAsync);

      SelectedFolderText = ApplicationSettings.RootFolder ?? FOLDER_NOT_SELECTED;
    }

    private void ShowComicInfo(Comic? selectedComic)
    {
      if (selectedComic != null)
      {
        _snackbarService.Show("Comic Info", $"Title: {selectedComic.Title}, Publisher: {selectedComic.Publisher}", ControlAppearance.Info, new SymbolIcon(SymbolRegular.Fluent24), TimeSpan.FromSeconds(10));
      }
    }

    private void SaveRootFolderAsync()
    {
      _settingsRepo.InsertOrUpdateSetting(ApplicationSettingKey.RootFolder, SelectedFolderText);
    }

    private async void ScanFolderAsync()
    {
      try
      {
        var files = await FolderHandler.ScanFolder(SelectedFolderText).ConfigureAwait(false); ;
        if (files == null || !files.Any()) return;


        var comics = files.Select(file => new Comic(file, _extractor)).ToList();

        var result = System.Windows.MessageBox.Show("Found " + comics.Count + " comics. Do you want to start scanning?", "Comics Found", System.Windows.MessageBoxButton.OKCancel);

        if (result == System.Windows.MessageBoxResult.OK)
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

    private async void PickFolderAsync()
    {
      try
      {
        var tempSelectedFolderText = SelectComicFolder;
        Loading = true;
        var folderName = SelectComicFolder();
        if (folderName == null)
        {
          SelectedFolderText = FOLDER_NOT_SELECTED;
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

    private async void OpenSelectedComicAsync()
    {
      var selectedComicId = SelectedComic;
      var selectedComic = this.ComicsCollection[selectedComicId];

      if (selectedComic == null) return;

      //var reader  = Application.Current.ServiceProvider.GetRequiredService<Reader>();
      var reader = _serviceProvider.GetRequiredService<Reader>();
      reader.SetUpAsync(selectedComic);
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

    private bool CanScanOrSave()
    {
      return SelectedFolderText != FOLDER_NOT_SELECTED;
    }

    partial void OnSelectedFolderTextChanged(string value)
    {
      ScanCommand.NotifyCanExecuteChanged();
      SaveRootFolderCommand.NotifyCanExecuteChanged();
    }

    private string ProgressText(int count, int index)
    {
      return $"Progress: {index}/{count}";
    }

  }
}