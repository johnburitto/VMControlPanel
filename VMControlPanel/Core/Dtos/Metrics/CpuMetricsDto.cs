namespace Core.Dtos.Metrics
{
    public class CpuMetricsDto
    {
        public List<float>? Times { get; set; }
        public CpuPercentageDto? CpuPercentage { get; set; }
        public List<long>? FileDesc { get; set;}
        public List<float>? LoadAvg { get; set;}
        public Dictionary<string, string>? CpuInfo { get; set; }
        public Dictionary<string, int>? ProcsNumber { get; set; }
    }
}
