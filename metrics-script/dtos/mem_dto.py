class MemMetricsDto:
    MemActive = 0.0
    MemTotal = 0.0 
    MemCached = 0.0
    MemFree = 0.0
    SwapTotal = 0.0 
    SwapFree = 0.0

    def __init__(self, mem_active, mem_total, mem_cached, mem_free, swap_total, swap_free) -> None:
        self.MemActive = mem_active
        self.MemTotal = mem_total
        self.MemCached = mem_cached
        self.MemFree = mem_free
        self.SwapTotal = swap_total
        self.SwapFree = swap_free