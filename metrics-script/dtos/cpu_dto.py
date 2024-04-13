class CpuMetricsDto:
    Times = []
    CpuPercentage = None
    ProcsNumber = {}
    FileDesc = []
    LoadAvg = []
    CpuInfo = {}

    def __init__(self) -> None:
        pass
    
    def to_dict(self):
        dict = self.__dict__

        dict['CpuPercentage'] = self.CpuPercentage.__dict__
        dict['ProcsNumber'] = self.ProcsNumber
        dict['CpuInfo'] = self.CpuInfo
        
        return dict

class CpuPercentageDto:
    User = 0.0
    Nice = 0.0
    System = 0.0
    Idle = 0.0
    Iowait = 0.0
    Irq = 0.0
    Softirq = 0.0

    def __init__(self, user, nice, system, idle, iowait, irq, softirq) -> None:
        self.User = user
        self.Nice = nice
        self.System = system
        self.Idle = idle
        self.Iowait = iowait
        self.Irq = irq
        self.Softirq = softirq
