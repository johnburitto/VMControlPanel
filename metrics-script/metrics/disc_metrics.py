import time
from subprocess import Popen, PIPE

def disk_busy(device, sample_duration=1):
    with open('/proc/diskstats') as first_diskstats:
        with open('/proc/diskstats') as second_diskstats:
            first_content = first_diskstats.read()
            time.sleep(sample_duration)
            second_content = second_diskstats.read()
    
    sep = '%s ' % device
    found = False

    for line in first_content.splitlines():
        if sep in line:
            found = True
            io_ms1 = line.strip().split(sep)[1].split()[9]

            break
        
    if not found:
            raise DiskError('Device not found: %r' % device)
        
    for line in second_content.splitlines():
        if sep in line:
            io_ms2 = line.strip().split(sep)[1].split()[9]

            break
        
    delta = int(io_ms2) - int(io_ms1)
    total = sample_duration * 1000        
        
    return 100 * (float(delta) / total)

def disk_reads_writes(device):
    with open('/proc/diskstats') as diskstats:
        content = diskstats.read()

    sep = '%s ' % device
    found = False

    for line in content.splitlines():
        if sep in line:
            found = True
            fields = line.strip().split(sep)[1].split()
            num_reads = int(fields[0])
            num_writes = int(fields[4])

            break
    
    if not found:
            raise DiskError('Device not found: %r' % device)
        
    return (num_reads, num_writes)

def disk_usage(path):
    output = Popen(['df', '-k', path], stdout=PIPE).communicate()[0]

    try:
        df = output.splitlines()[2].split()
        device = output.splitlines()[1]
        (size, used, free, percent, mountpoint) = df
    except IndexError:
        df = output.splitlines()[1].split()
        (device, size, used, free, percent, mountpoint) = df
    
    return (device, int(size), int(used), int(free), percent, mountpoint)

def disk_reads_writes_persec(device, sample_duration=1):
    with open('/proc/diskstats') as first_diskstats:
        with open('/proc/diskstats') as second_diskstats:
            first_content = first_diskstats.read()
            time.sleep(sample_duration)
            second_content = second_diskstats.read()
    
    sep = '%s ' % device
    found = False

    for line in first_content.splitlines():
        if sep in line:
            found = True
            fields = line.strip().split(sep)[1].split()
            first_num_reads = int(fields[0])
            first_num_writes = int(fields[4])

            break
    
    if not found:
        raise DiskError('Device not found: %r' % device)
    
    for line in second_content.splitlines():
        if sep in line:
            fields = line.strip().split(sep)[1].split()
            second_num_reads = int(fields[0])
            second_num_writes = int(fields[4])

    reads_persec = (second_num_reads - first_num_reads) / float(sample_duration)
    writes_persec = (second_num_writes - first_num_writes) / float(sample_duration)

    return (reads_persec, writes_persec)

class DiskError(Exception):
    pass