namespace Core.Dtos
{
    public class FileDto
    {
        public Stream FileStream { get; set; } = Stream.Null;
        public string? FileName { get; set; }
        public string? Message { get; set; }
        public bool IsUploaded => FileStream != null;
    }
}
