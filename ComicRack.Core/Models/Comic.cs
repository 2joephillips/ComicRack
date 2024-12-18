using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComicRack.Core.Models;

[Table("Comics")]
public class Comic
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public Guid IdGuid { get; set; }

    public string FilePath { get; set; }
    public string FileName { get; set; }

    [NotMapped]
    public int ImageCount { get; set; }

    [NotMapped]
    public (string ThumbnailPath, string MediumPath, string HighResPath) CoverImagePaths { get; set; }

    [NotMapped]
    public MetaData MetaData { get; set; }
    [NotMapped]
    public bool UnableToOpen { get; set; }

    [NotMapped]
    public string GetHighResImagePath => CoverImagePaths.HighResPath;

    private readonly IComicMetadataExtractor _metadataExtractor;

    public Comic() { }

    public Comic(string filePath, IComicMetadataExtractor metadataExtractor)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        IdGuid = Guid.NewGuid();
        FilePath = filePath;
        FileName = filePath.Split('\\').LastOrDefault() ?? string.Empty;
        _metadataExtractor = metadataExtractor ?? throw new ArgumentNullException(nameof(metadataExtractor));
    }

    public void LoadMetaData()
    {
        try
        {
            (MetaData metaData, int imageCount, (string ThumbnailPath, string MediumPath, string HighResPath) coverPaths) = _metadataExtractor.ExtractMetadata(FilePath);
            MetaData = metaData;
            CoverImagePaths = coverPaths;
            ImageCount = imageCount;
        }
        catch (Exception)
        {
            UnableToOpen = true;
        }
    }
}
