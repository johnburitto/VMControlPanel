class DiskMetricsDto:
    DiskBusy = {}
    DiskReadsWrites = {}
    DiskReadsWritesPersec = {}
    DiskUsage = {}

    def __init__(self) -> None:
        pass

    def to_dict(self):
        dict = self.__dict__

        dict['DiskBusy'] = self.DiskBusy
        dict['DiskReadsWrites'] = self.DiskReadsWrites
        dict['DiskReadsWritesPersec'] = self.DiskReadsWritesPersec
        dict['DiskUsage'] = self.DiskUsage
        
        return dict