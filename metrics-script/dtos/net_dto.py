class NetMetricsDto:
    RxTxBytes = {}
    RxTxBits = {}
    RxTxDump = {}
    NetStatsIfconfig = {}

    def __init__(self) -> None:
        pass

    def to_dict(self):
        dict = self.__dict__

        dict['RxTxBytes'] = self.RxTxBytes
        dict['RxTxBits'] = self.RxTxBits
        dict['RxTxDump'] = self.RxTxDump
        dict['NetStatsIfconfig'] = self.NetStatsIfconfig
        
        return dict