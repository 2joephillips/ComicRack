
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

    public StartUpViewModel()
    {
      
    }
    public StartUpViewModel(IComicMetadataExtractor extractor, IServiceProvider serviceProvider, ISettingsRepository settingsRepo, ISnackbarService snackbarService)
    {
      _extractor = extractor;
      _serviceProvider = serviceProvider;
      _settingsRepo = settingsRepo;
      _snackbarService = snackbarService;


      PickFolderCommand = new RelayCommand(PickFolderAsync);
      SaveRootFolderCommand = new RelayCommand(SaveRootFolderAsync, CanScanOrSave);
      ScanCommand = new RelayCommand(ScanFolderAsync, CanScanOrSave);
      ShowInfoCommand = new RelayCommand<Comic>(ShowComicInfo);
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
      _snackbarService.Show("Root Folder Saved", $"{SelectedFolderText} saved as Root Folder",ControlAppearance.Info ,new SymbolIcon(SymbolRegular.Save24), TimeSpan.FromSeconds(5));
    }


    private async void PickFolderAsync()
    {
      try
      {
        var tempSelectedFolderText = SelectedFolderText;
        Loading = true;
        var folderName = await SelectComicFolder();
        if (folderName != null)
        {
          SelectedFolderText = folderName;
          return;
        }

        SelectedFolderText = tempSelectedFolderText;
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

        var result = System.Windows.MessageBox.Show("Found " + comics.Count + " comics. Do you want to start scanning?", "Comics Found", System.Windows.MessageBoxButton.OKCancel);

        if (result == System.Windows.MessageBoxResult.OK)
        {
          var index = 1;
          ScanningProgress = ProgressText(index, comics);
          foreach (var comic in comics)
          {
            ScanningProgress = ProgressText(index++, comics);
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

    private async Task<string?> SelectComicFolder()
    {
      return await Task.Run(() =>
      {
        var folderDialog = new OpenFolderDialog();
        var result = folderDialog.ShowDialog() ?? false;
        return result ? folderDialog.FolderName : null;
      });
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

    private string ProgressText(int index, List<Comic> comics)
    {
      if (index < comics.Count())
        return $"{index}/{comics.Count()} Scanning: {comics[index - 1].FileName}";
      else
        return $"{index}/{comics.Count()} Unable To open: {comics.Count(s =>s.UnableToOpen)} Needs Metadata : {comics.Count(s => s.NeedsMetaData)} ";
    }

  }
}