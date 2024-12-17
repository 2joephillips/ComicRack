namespace ComicRack.Core;

public static class FolderHandler
{
    public static Task<List<string>> ScanFolder(string folderName)
    {
        return Task.Run(() =>
        {
            var supportedExtensions = new List<string>() { ".jpg", ".png", ".pdf", ".cbz", ".cbr" };

            // Get all file paths in the root directory and its subdirectories
            var filePaths = Directory.GetFiles(folderName, "*.*", SearchOption.AllDirectories)
                .Where(file => supportedExtensions.Contains(Path.GetExtension(file).ToLower()))
                .ToList();

            return filePaths;
        });
    }

   
}
