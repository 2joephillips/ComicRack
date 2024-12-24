
using ComicRack.Core;
using ComicRack.Core.Models;
using System.IO.Compression;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ComicRack.Desktop.Views.Windows
{
    public partial class ReaderViewModel : ObservableObject
    {
        private Comic _comic;
        private ZipArchive _zipArchive;  // Keep reference to ZipArchive
        private Dictionary<int, ZipArchiveEntry> _images = new();
        private int _activeImageIndex;

        [ObservableProperty]
        private SolidColorBrush calculatedBackgroundColor = new(Colors.Black);

        [ObservableProperty]
        private SolidColorBrush calculatedHoverBackgroundColor = new SolidColorBrush(Color.FromArgb(191, 0, 0, 0)); // Black with 75% opacity

        [ObservableProperty]
        private BitmapImage currentPageImage = new();

        public IRelayCommand NextPageCommand { get; }
        public IRelayCommand PreviousPageCommand { get; }

        public ReaderViewModel()
        {
            NextPageCommand = new RelayCommand(() => LoadPage(1));
            PreviousPageCommand = new RelayCommand(() => LoadPage(-1));
        }

        public async Task SetUpComicAsync(Comic comic)
        {
            _comic = comic;
            await ParseImagesAsync();
            CurrentPageImage = BitmapImageHandler.LoadBitmapImageFromPath(_comic.CoverImagePaths.HighResPath);
            CalculatedBackgroundColor = ColorAnalyzer.GetTopColor(_comic.CoverImagePaths.ThumbnailPath);
            CalculatedHoverBackgroundColor = new SolidColorBrush(Color.FromArgb(150, CalculatedBackgroundColor.Color.R, CalculatedBackgroundColor.Color.G, CalculatedBackgroundColor.Color.B));

        }

        private void LoadPage(int step)
        {
            _activeImageIndex = (_activeImageIndex + step + _images.Count) % _images.Count;
            var entry = _images[_activeImageIndex];
            var image = ImageHandler.GetImageFromZipArchiveEntry(entry);
            CurrentPageImage= BitmapImageHandler.CreateBitmapImageFromImage(image);

        }
        private async Task ParseImagesAsync()
        {
            _images.Clear();

            // Open the ZipArchive for reading and store it in a class-level variable
            _zipArchive = ZipFile.OpenRead(_comic.FilePath);  // Keep reference to ZipArchive

            var supportedExtensions = new[] { ".jpg", ".png" };

            // Filter out the entries that don't match supported image extensions
            var filtered = _zipArchive.Entries
                .Where(entry => (supportedExtensions.Any(e => entry.Name.ToLower().EndsWith(e))))
                .ToList();

            var indexedItems = filtered.Select((item, index) => new { Index = index, Item = item })
                .ToList();

            // Add the filtered and indexed entries to the _images dictionary
            foreach (var item in indexedItems)
            {
                _images.Add(item.Index, item.Item);
            }
        }
    }
}