from metrics import cpu_metrics
from metrics import disk_metrics
from metrics import mem_metrics
from metrics import net_metrics
from dtos import metrics_dto

if __name__ == "__main__":
    dto = metrics_dto.MetricsDto()

    # cpu
    dto.CpuDto.Times = cpu_metrics.cpu_times()
    dto.CpuDto.CpuPercentage = cpu_metrics.cpu_percentage()
    dto.CpuDto.ProcsNumber['procs_running'] = cpu_metrics.procs_running()
    dto.CpuDto.ProcsNumber['procs_blocked'] = cpu_metrics.procs_blocked()
    dto.CpuDto.FileDesc = cpu_metrics.file_desc()
    dto.CpuDto.LoadAvg = cpu_metrics.load_avg()
    dto.CpuDto.CpuInfo = cpu_metrics.cpu_info()

    # disk
    for disc in disk_metrics.get_discs():
        dto.DiscDto.DiskBusy[disc] = disk_metrics.disk_busy(disc)
        dto.DiscDto.DiskReadsWrites[disc] = disk_metrics.disk_reads_writes(disc)
        dto.DiscDto.DiskReadsWritesPersec[disc] = disk_metrics.disk_reads_writes_persec(disc)
        #dto.disc_dto.disk_usage[disc] = disk_metrics.disk_usage(disc)

    # memory
    dto.MemDto = mem_metrics.mem_metrics()

    # network
    for interface in net_metrics.get_interfaces():
        dto.NetDto.RxTxBytes[interface] = net_metrics.rx_tx_bytes(interface)
        dto.NetDto.RxTxBits[interface] = net_metrics.rx_tx_bits(interface)
        dto.NetDto.RxTxDump[interface] = net_metrics.rt_xt_dump(interface)
        #dto.net_dto.net_stats_ifconfig[interface] = net_metrics.net_stats_ifconfig(interface)

    print(dto.to_dict())