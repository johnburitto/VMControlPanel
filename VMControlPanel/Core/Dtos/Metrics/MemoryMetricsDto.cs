namespace Core.Dtos.Metrics
{
    public class MemoryMetricsDto
    {
        public float MemActive { get; set; }
        public float MemTotal { get; set; }
        public float MemCached { get; set; }
        public float MemFree { get; set; }
        public float SwapTotal { get; set; }
        public float SwapFree { get; set; }
    }
}
