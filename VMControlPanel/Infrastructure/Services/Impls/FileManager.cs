using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services.Impls
{
    public static class FileManager
    {
        public static string FileDirectory => $"{Directory.GetCurrentDirectory()}/Files";

        public static void CreateDirectory()
        {
            if (!Directory.Exists(FileDirectory))
            {
                Directory.CreateDirectory(FileDirectory);
            }
        }

        public static void DeleteFile(string path)
        {
            if (path.IsNullOrEmpty())
            {
                return;
            }

            File.Delete(path);
        }

        public static Stream OpenFileAsStream(string path)
        {
            return File.OpenRead(path);
        }
    }
}
