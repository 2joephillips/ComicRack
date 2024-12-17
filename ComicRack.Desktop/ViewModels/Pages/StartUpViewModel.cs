
namespace ComicRack.Desktop.ViewModels.Pages
{
    public class StartUpViewModel
    {
        //private async void ScanFolders(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        var folderName = SelectComicFolder();
        //        if (folderName == null) return;

        //        var files = await FolderHandler.ScanFolder(folderName).ConfigureAwait(false); ;
        //        if (files == null || !files.Any()) return;

        //        var comics = files.Select(file => new Comic(file, _extractor)).ToList();

        //        var result = MessageBox.Show("Found " + comics.Count + " comics. Do you want to start scanning?", "?", MessageBoxButton.OKCancel);
        //        if (result == MessageBoxResult.OK)
        //        {
        //            await Dispatcher.InvokeAsync(() =>
        //            {
        //                progress_bar.Maximum = comics.Count;
        //            });

        //            foreach (var comic in comics)
        //            {
        //                // Fetch metadata for the comic on a background thread
        //                await Task.Run(() => comic.LoadMetaData()).ConfigureAwait(false);

        //                await Dispatcher.InvokeAsync(() =>
        //                {
        //                    progress_bar.Value++;
        //                    UpdateTreeView(comic, comics.Count);
        //                });

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Initialization failed: {ex.Message}");
        //        Close();
        //    }
        //}

        //private void UpdateTreeView(Comic comic, int totalCount)
        //{
        //    // Create a TreeViewItem for the comic and update the UI
        //    var item = CreateTreeViewItem(comic.UnableToOpen, comic.FilePath, comic.CoverImagePaths.ThumbnailPath);
        //    comics_list.Items.Add(item);

        //    if (!comic.UnableToOpen)
        //    {
        //        var bitmapImage = new BitmapImage();
        //        bitmapImage.BeginInit();
        //        bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
        //        bitmapImage.UriSource = new Uri(comic.CoverImagePaths.HighResPath, UriKind.Absolute);
        //        bitmapImage.EndInit();
        //        loading_image.Source = bitmapImage;
        //    }

        //    statusLabel.Text = $"Processed {comics_list.Items.Count}/{totalCount} comics.";

        //}

        //private object CreateTreeViewItem(bool unableToOpen, string fileName, string imagePath)
        //{
        //    var stackPanel = new StackPanel
        //    {
        //        Orientation = Orientation.Horizontal
        //    };


        //    //var iconShape = unableToOpen ? FontAwesomeIcon.ExclamationCircle : FontAwesomeIcon.CheckCircle;
        //    //var iconColor = unableToOpen ? Brushes.Red : Brushes.Green;

        //    //var iconSource = ImageAwesome.CreateImageSource(iconShape, iconColor);
        //    //var icon = new System.Windows.Controls.Image
        //    //{
        //    //    Source = iconSource,
        //    //    Width = 16,  // Set the desired width
        //    //    Height = 16, // Set the desired height
        //    //    Margin = new Thickness(0, 0, 5, 0) // Add spacing if needed
        //    //};
        //    //stackPanel.Children.Add(icon);

        //    var textBlock = new TextBlock
        //    {
        //        Text = fileName,
        //        VerticalAlignment = VerticalAlignment.Center
        //    };
        //    stackPanel.Children.Add(textBlock);

        //    // Create TreeViewItem and set its Header
        //    var treeViewItem = new TreeViewItem
        //    {
        //        Margin = new Thickness(1),
        //        Header = stackPanel
        //    };
        //    if (!unableToOpen)
        //    {
        //        var bitmapImage = new BitmapImage();
        //        bitmapImage.BeginInit();
        //        bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
        //        bitmapImage.UriSource = new Uri(imagePath, UriKind.Absolute);
        //        bitmapImage.EndInit();
        //        treeViewItem.ToolTip = new ToolTip
        //        {
        //            Content = new System.Windows.Controls.Image
        //            {
        //                Width = 300,
        //                Height = 200,
        //                Source = bitmapImage
        //            }
        //        };
        //    }

        //    return treeViewItem;
        //}

        //private void SaveFilesToDB(List<Comic> files)
        //{
        //    _dbContext.Comics.AddRangeAsync(files);
        //    _dbContext.SaveChanges();
        //}

        //private string? SelectComicFolder()
        //{
        //    var folderDialog = new OpenFolderDialog();
        //    if (folderDialog.ShowDialog() == true)
        //    {
        //        return folderDialog.FolderName;
        //    }

        //    return null;
        //}

    }
}