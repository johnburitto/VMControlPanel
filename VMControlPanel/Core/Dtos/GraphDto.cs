namespace Core.Dtos
{
    public class GraphDto
    {
        public string? Name { get; set; }
        public string? Title { get; set; }
        public int Width { get; set; } = 1080;
        public int Height { get; set; } = 720;
        public List<string>? Labels { get; set; }
        public List<float>? Values { get; set; }
        public string? UserId { get; set; }
    }
}
