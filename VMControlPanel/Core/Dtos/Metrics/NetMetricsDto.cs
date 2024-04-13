namespace Core.Dtos.Metrics
{
    public class NetMetricsDto
    {
        public Dictionary<string, List<int>>? RxTxBytes { get; set; }
        public Dictionary<string, List<int>>? RxTxBits { get; set; }
        public Dictionary<string, List<List<int>>>? RxTxDump { get; set; }
        public object? NetStatsIfconfig { get; set; }
    }
}
