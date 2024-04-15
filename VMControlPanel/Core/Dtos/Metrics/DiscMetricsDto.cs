namespace Core.Dtos.Metrics
{
    public class DiscMetricsDto
    {
        public Dictionary<string, float>? DiskBusy { get; set; }
        public Dictionary<string, List<int>>? DiskReadsWrites { get; set; }
        public Dictionary<string, List<float>>? DiskReadsWritesPersec { get; set; }
        public object? DiskUsage { get; set; }
    }
}
