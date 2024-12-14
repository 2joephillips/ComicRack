using System.IO.Compression;
using System.Xml.Linq;

namespace ComicRack.Core;

public interface IComicMetadataExtractor
{
    (MetaData metaData, string coverImagePath) ExtractMetadata(string filePath);
}


public class ComicMetadataExtractor : IComicMetadataExtractor
{
    private readonly ISystemStorage _storage;
    private static readonly string[] ImageExtensions = { "jpg", "jpeg", "png", "JPG", "JPEG", "PMG" };

    public ComicMetadataExtractor(ISystemStorage storage)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public (MetaData metaData, string coverImagePath) ExtractMetadata(string filePath)
    {
        if (!filePath.EndsWith(".cbz", StringComparison.OrdinalIgnoreCase))
            throw new NotSupportedException("Unsupported file type");

        using FileStream zipToOpen = new FileStream(filePath, FileMode.Open);
        using var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read);

        var comicXML = archive.Entries.FirstOrDefault(e => e.Name == "ComicInfo.xml");
        var coverImage = archive.Entries.FirstOrDefault(e => ImageExtensions.Any(e.Name.Contains));

        MetaData metaData = null;

        if (comicXML != null)
        {
            using var stream = comicXML.Open();
            var doc = XDocument.Load(stream);
            metaData = new MetaData(doc);
        }
        string coverImagePath = coverImage != null ? _storage.WriteComicCoverToAppData(coverImage) ?? string.Empty : string.Empty;
        return (metaData, coverImagePath);
    }
}
