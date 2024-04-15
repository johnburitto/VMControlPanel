from .cpu_dto import *
from .disk_dto import *
from .mem_dto import *
from .net_dto import *

class MetricsDto:
    CpuDto = None
    DiscDto = None
    MemDto = None
    NetDto = None

    def __init__(self) -> None:
        self.CpuDto = CpuMetricsDto()
        self.DiscDto = DiskMetricsDto()
        self.NetDto = NetMetricsDto()
    
    def to_dict(self):
        dict = self.__dict__

        dict['CpuDto'] = self.CpuDto.to_dict()
        dict['DiscDto'] = self.DiscDto.to_dict()
        dict['MemDto'] = self.MemDto.__dict__
        dict['NetDto'] = self.NetDto.to_dict()
        
        return dict
