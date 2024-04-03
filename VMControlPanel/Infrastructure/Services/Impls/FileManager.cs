using Infrastructure.Services.Interfaces;

namespace Infrastructure.Services.Impls
{
    public static class FileManager
    {
        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public static Stream OpenFileAsStream(string path)
        {
            return File.OpenRead(path);
        }
    }
}
