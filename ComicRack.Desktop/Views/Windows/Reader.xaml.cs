using ComicRack.Core.Models;
using System.IO.Compression;

namespace ComicRack.Desktop.Views.Windows
{
    public partial class Reader : Window
    {
        public ReaderViewModel ViewModel { get; }

        public Reader(ReaderViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = this;
        }
        
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NextPageCommand.Execute(this);
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.PreviousPageCommand.Execute(this);
        }

        internal async Task SetUpAsync(Comic selectedComic)
        {
            await ViewModel.SetUpComicAsync(selectedComic);
        }
    }
}
