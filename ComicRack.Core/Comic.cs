using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Compression;
using System.Xml.Linq;

namespace ComicRack.Core;


[Table("Comics")]
public class Comic
{
    private string file;

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string FilePath { get; set; }
    public string FileName { get; set; }

    [NotMapped]
    public MetaData MetaData { get; set; }
    [NotMapped]
    public bool UnableToOpen { get; set; }

    public Comic() { }

    public Comic(string file)
    {
        FilePath = file;
        FileName = file.Split('\\').LastOrDefault() ?? string.Empty;
    }

    public void GetMetaData()
    {
        if (FilePath.EndsWith(".cbz", StringComparison.OrdinalIgnoreCase))
        {
            ExtractMetadataFromZip();
        }
        else
        {
            throw new NotImplementedException("Unsupported File Type");
        }
    }

    private void ExtractMetadataFromZip()
    {
        try
        {
            using FileStream zipToOpen = new FileStream(FilePath, FileMode.Open);
            using ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read);
            var comicXML = archive.Entries.FirstOrDefault(e => e.Name == "ComicInfo.xml");
            if (comicXML != null)
            {
                using Stream stream = comicXML.Open();
                var doc = XDocument.Load(stream);
                var comicInfo = new MetaData(doc);
                MetaData = comicInfo;
            }
        }
        catch (Exception ex)
        {
            UnableToOpen = true;
        }
    }
}
