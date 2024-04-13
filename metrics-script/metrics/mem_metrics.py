from dtos import mem_dto

def mem_metrics():
    with open('/proc/meminfo') as meminfo:
        for line in meminfo:
            if line.startswith('MemTotal:'):
                mem_total = float(line.split()[1]) / 1024
            elif line.startswith('Active:'):
                mem_active = float(line.split()[1]) / 1024
            elif line.startswith('MemFree:'):
                mem_free = float(line.split()[1]) / 1024
            elif line.startswith('Cached:'):
                mem_cached = float(line.split()[1]) / 1024
            elif line.startswith('SwapTotal:'):
                swap_total = float(line.split()[1]) / 1024
            elif line.startswith('SwapFree:'):
                swap_free = float(line.split()[1]) / 1024
    
    return mem_dto.MemMetricsDto(mem_active, mem_total, mem_cached, mem_free, swap_total, swap_free)