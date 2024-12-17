using ComicRack.Core.Models;
using System.IO.Compression;
using System.Xml.Linq;

namespace ComicRack.Core;
public interface IComicMetadataExtractor
{
    (MetaData metaData, int imageCount, (string ThumbnailPath, string MediumPath, string HighResPath) coverImagePath) ExtractMetadata(string filePath);
}
public class ComicMetadataExtractor : IComicMetadataExtractor
{
    private readonly ISystemStorage _storage;
    string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

    public ComicMetadataExtractor(ISystemStorage storage)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public (MetaData metaData,int imageCount, (string ThumbnailPath, string MediumPath, string HighResPath) coverImagePath) ExtractMetadata(string filePath)
    {
        if (!filePath.EndsWith(".cbz", StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException("Unsupported file type");

        using FileStream zipToOpen = new FileStream(filePath, FileMode.Open);
        using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read);

        var comicXML = archive.Entries.FirstOrDefault(e => e.Name == "ComicInfo.xml");
        var coverImage = archive.Entries.FirstOrDefault(e => imageExtensions.Any(e.Name.Contains));

        var subdirectories = archive.Entries
            .Where(entry => entry.FullName.Contains("/") && entry.Length > 0) // Entries with "/" are subdirectories
            .Select(entry => Path.GetDirectoryName(entry.FullName))
            .Distinct()
            .ToList();

        int imageCount = archive.Entries
              .Count(entry => imageExtensions.Any(ext => entry.FullName.EndsWith(ext, StringComparison.OrdinalIgnoreCase)));

        MetaData metaData = null;
        
        if (comicXML != null)
        {
            using var stream = comicXML.Open();
            var doc = XDocument.Load(stream);
            metaData = new MetaData(doc);
        }
        var coverImagePaths = _storage.CreateComicCoverImages(coverImage);
        return (metaData, imageCount, coverImagePaths);
    }
}
