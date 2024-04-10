from metrics import cpu_metrics
from metrics import disc_metrics
from metrics import mem_metrics
from metrics import net_metrics

if __name__ == "__main__":
    # cpu
    print('procs running: %d' % cpu_metrics.procs_running())
    cpu_pcts = cpu_metrics.cpu_percentage(sample_duration=1)
    print('cpu utilization: %.2f%%' % (100 - cpu_pcts['idle']))

    # disk
    print('disk busy: %s%%' % disc_metrics.disk_busy('sda', sample_duration=1))
    r, w = disc_metrics.disk_reads_writes('sda')
    print('disk reads: %s' % r)
    print('disk writes: %s' % w)

    # memory
    used, total, _, _, _, _ = mem_metrics.mem_metrics()
    print('mem used: %s' % used)
    print('mem total: %s' % total)

    # network
    rx_bits, tx_bits = net_metrics.rx_tx_bits('eth0')
    print('net bits received: %s' % rx_bits)
    print('net bits sent: %s' % tx_bits)