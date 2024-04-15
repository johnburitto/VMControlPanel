namespace Core.Dtos.Metrics
{
    public class CpuPercentageDto
    {
        public float User {  get; set; }
        public float Nice {  get; set; }
        public float System {  get; set; }
        public float Idle {  get; set; }
        public float Iowait {  get; set; }
        public float Irq {  get; set; }
        public float Softirq {  get; set; }
    }
}
