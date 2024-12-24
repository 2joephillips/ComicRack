using ComicRack.Core;
using ComicRack.Core.Models;
using System.Drawing.Imaging;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using ComicRack.Desktop.ViewModels.Windows;

namespace ComicRack.Desktop.Views.Windows
{

    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double originalValue && double.TryParse(parameter?.ToString(), out double percentage))
            {
                return originalValue * percentage;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Interaction logic for Reader.xaml
    /// </summary>
    public partial class Reader : Window
    {
        private Comic _comic;
        private Dictionary<int, ZipArchiveEntry> _images = new Dictionary<int, ZipArchiveEntry>();
        private int _activeImageIndex = 0;

        public ReaderViewModel ViewModel { get; }

        public Reader(ReaderViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }
        //public Reader(Comic selectedComic)
        //{
        
        //}

        private void ParseImages()
        {
            Dictionary<int, ZipArchiveEntry> pages = new Dictionary<int, ZipArchiveEntry>();

            // Assuming cbz is a ZIP archive
            System.IO.Compression.ZipArchive zipArchive = System.IO.Compression.ZipFile.OpenRead(_comic.FilePath);
            var supportedExtensions = new List<string>() { ".jpg", ".png", ".pdf", ".cbz", ".cbr" };
            var filtered = zipArchive.Entries
                 .Where(entry => (supportedExtensions.Any(e => entry.Name.ToLower().Contains(e)))).ToList();
             var indexedItems = filtered.Select((item, index) => new { Index = index, Item = item })
                .ToList();
            foreach (var entry in indexedItems)
            {
                    _images.Add(entry.Index,entry.Item);
            }
        }

        private void DisplayCoverImage() {

                // Load image from the zip file (temp extraction might be required)
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(_comic.CoverImagePaths.HighResPath, UriKind.Absolute); // Load from path
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                ComicPageImage.Source = bitmap;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_activeImageIndex == _images.Count()-1)
            {
                _activeImageIndex = 0;
            } else
                _activeImageIndex++;

           
            ComicPageImage.Source = BuildImage();
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (_activeImageIndex == 0)
            {
                _activeImageIndex = _images.Count()-1;
            }
            else
                _activeImageIndex--;


            ComicPageImage.Source = BuildImage();
        }
        private BitmapImage BuildImage()
        {
            var image = ImageHandler.GetImageFromZipArchiveEntry(_images[_activeImageIndex]);
            BitmapImage bitmap = new();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Save the System.Drawing.Image to the MemoryStream
                image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                // Reset the stream position to the beginning
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Load the BitmapImage from the stream
                bitmap.BeginInit();
                bitmap.StreamSource = memoryStream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Load image immediately
                bitmap.EndInit();
            }

            return bitmap;
        }

        internal void SetUp(Comic selectedComic)
        {
            _comic = selectedComic;
            ParseImages();
            DisplayCoverImage();
            using Bitmap bitmap1 = new Bitmap(selectedComic.CoverImagePaths.ThumbnailPath);
            var topColors = ColorAnalyzer.GetTopColors(bitmap1, 3);
            // Set the most frequent color as the background
            if (topColors.Any())
            {
                var topColor = topColors.Last();
                this.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(topColor.R, topColor.G, topColor.B));
            }

        }
    }
}
