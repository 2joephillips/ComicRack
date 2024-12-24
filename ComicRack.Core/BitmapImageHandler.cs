using System.IO;
using System.IO.Compression;
using System.Windows.Media.Imaging;

namespace ComicRack.Core;

public static class BitmapImageHandler
{

    public static BitmapImage LoadBitmapImageFromPath(string path)
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.UriSource = new Uri(path, UriKind.Absolute);
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.EndInit();
        return bitmap;
    }

    public static BitmapImage CreateBitmapImageFromImage(System.Drawing.Image image)
    {
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
}
