using System.Text;
using System.Text.Json;
using ComicReader.Core;


var comicRack = new ComicRack();
var storageUrl = comicRack.STORAGE_URL;
var library = comicRack.COMIC_LIBRARY_LOCACTION;
 
try
{





}
catch (UnauthorizedAccessException ex)
{
  Console.WriteLine($"Access denied to a directory: {ex.Message}");
}
catch (Exception ex)
{
  Console.WriteLine($"An error occurred: {ex.Message}");
}

