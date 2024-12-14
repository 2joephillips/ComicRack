using System.IO.Compression;

namespace ComicRack.Core
{
    public interface ISystemStorage
    {
        string AppDataPath { get; }

        string WriteComicCoverToAppData(ZipArchiveEntry? coverImage);
    }

    public class SystemStorage : ISystemStorage
    {
        public string AppDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ComicRack");
        public SystemStorage()
        {
            EnsureAppDataFolderExists();
        }

        public string WriteComicCoverToAppData(ZipArchiveEntry? coverImage)
        {
            if (coverImage == null)
                return "";

            var fileName = Guid.NewGuid().ToString() + "." + coverImage.Name.Split('.').LastOrDefault();
            var filePath = Path.Combine(AppDataPath, fileName);
            using var entryStream = coverImage.Open();
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

            entryStream.CopyTo(fileStream);

            Console.WriteLine($"File extracted to: {filePath}");

            return filePath;
        }

        private void EnsureAppDataFolderExists()
        {
            if (!Directory.Exists(AppDataPath))
            {
                Directory.CreateDirectory(AppDataPath);
            }
        }

    }
}
