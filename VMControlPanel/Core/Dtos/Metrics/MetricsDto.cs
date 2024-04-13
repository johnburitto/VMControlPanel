namespace Core.Dtos.Metrics
{
    public class MetricsDto
    {
        public CpuMetricsDto? CpuDto { get; set; }
        public DiscMetricsDto? DiscDto { get; set; }
        public MemoryMetricsDto? MemDto { get; set; }
        public NetMetricsDto? NetDto { get; set; }
    }
}
