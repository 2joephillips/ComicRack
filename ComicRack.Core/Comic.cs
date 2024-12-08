using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    public Comic(){}

    public Comic(string file)
    {
        FilePath = file;
        FileName = file.Split('\\').LastOrDefault() ?? string.Empty;
    }
}
