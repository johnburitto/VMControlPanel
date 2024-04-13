import time
from dtos import cpu_dto

def cpu_times():
    with open('/proc/stat') as stat:
        line = stat.readline()

    cpu_times = [int(x) for x in line.split()[1:]]

    return cpu_times

def cpu_percentage(sample_duration=1):
    deltas = cpu_times_deltas(sample_duration)
    total = sum(deltas)
    percents = [100 - (100 * (float(total - x)/ total)) for x in deltas]

    return cpu_dto.CpuPercentageDto(percents[0], percents[1], percents[2], percents[3], percents[4], percents[5], percents[6])

def procs_running():
    return proc_stat('procs_running')

def procs_blocked():
    return proc_stat('procs_blocked')

def file_desc():
    with open('/proc/sys/fs/file-nr') as file_nr:
        line = file_nr.readline()
    
    return [int(x) for x in line.split()]

def load_avg():
    with open('/proc/loadavg') as loadavg:
        line = loadavg.readline()

    return [float(x) for x in line.split()[:3]]

def cpu_info():
    with open('/proc/cpuinfo') as cpuinfo_file:
        cpuinfo = {'processor_count': '0'}

        for line in cpuinfo_file:
            if ':' in line:
                fields = line.replace('\t', '').strip().split(': ')

                if fields[0] == 'processor':
                    cpuinfo['processor_count'] = str(int(cpuinfo['processor_count']) + 1)
                elif fields[0] != 'code id':
                    try:
                        cpuinfo[fields[0]] = fields[1]
                    except IndexError:
                        pass
        
    return cpuinfo

def cpu_times_deltas(sample_duration):
    with open('/proc/stat') as first_stat:
        with open('/proc/stat') as second_stat:
            first_line = first_stat.readline()

            time.sleep(sample_duration)

            second_line = second_stat.readline()
    
    deltas = [int(b) - int(a) for a, b in zip(first_line.split()[1:],
                                              second_line.split()[1:])]

    return deltas

def proc_stat(stat_name):
    with open('/proc/stat') as stat:
        for line in stat:
            if line.startswith(stat_name):
                return int(line.split()[1])