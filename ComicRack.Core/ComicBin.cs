namespace ComicRack.Core;

public class ComicBin
{
  private string comic_library_location = @"D:\Comics";
  private string storage_file_location = @"D:\Comics\";
  private string storage_file_name = "storage.json";

  public string STORAGE_URL => storage_file_location + storage_file_name;

  public string COMIC_LIBRARY_LOCACTION => comic_library_location;
  public List<Comic> Comics { get; set; }
}