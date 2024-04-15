namespace Core.Dtos
{
    public class FileDto
    {
        public string? FilePath { get; set; }
        public string? Message { get; set; }
        public bool IsUploaded { get; set; } = false;
    }
}
