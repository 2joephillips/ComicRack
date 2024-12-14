using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Compression;
using System.Xml.Linq;

namespace ComicRack.Core;

[Table("Comics")]
public class Comic 
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string FilePath { get; set; }
    public string FileName { get; set; }

    [NotMapped]
    public string CoverImagePath { get; set; }

    [NotMapped]
    public MetaData MetaData { get; set; }
    [NotMapped]
    public bool UnableToOpen { get; set; }

    private readonly IComicMetadataExtractor _metadataExtractor;

    public Comic() { }

    public Comic(string filePath, IComicMetadataExtractor metadataExtractor)
    {
        if(string.IsNullOrEmpty(filePath)) 
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        FilePath = filePath;
        FileName = filePath.Split('\\').LastOrDefault() ?? string.Empty;
        _metadataExtractor = metadataExtractor ?? throw new ArgumentNullException(nameof(metadataExtractor));
    }

    public void LoadMetaData()
    {
        try
        {
            (MetaData metaData, string coverPath) = _metadataExtractor.ExtractMetadata(FilePath);
            MetaData = metaData;
            CoverImagePath = coverPath;
        }
        catch (Exception)
        {
            UnableToOpen = true;
        }
    }
}
