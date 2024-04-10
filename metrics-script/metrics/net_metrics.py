import re
from subprocess import Popen, PIPE

def rx_tx_bytes(interface):
    with open('/proc/net/dev') as net_stat:
        for line in net_stat.readlines():
            if (interface in line):
                data = line.split('%s:' % interface)[1].split()
                rx_bytes, tx_bytes = (int(data[0]), int(data[8]))

                return (rx_bytes, tx_bytes)
    
    raise NetError('Interface not found: %r' % interface)

def rx_tx_bits(interface):
    rx_bytes, tx_bytes = rx_tx_bytes(interface)
    rx_bits = rx_bytes * 8
    tx_bits = tx_bytes * 8

    return (rx_bits, tx_bits)

def rt_xt_dump(interface):
    with open('/proc/net/dev') as net_stat:
        for line in net_stat.readlines():
            if interface in line:
                data = line.split('%s:' % interface)[1].split()
                rx, tx = [int(x) for x in data[0:8]], [int(x) for x in data[8:]]

    return (rx, tx)

def net_stats_ifconfig(interface):
    output = Popen(['ifconfig', interface], stdout=PIPE).communicate()[0]
    rx_bytes = int(re.findall('RX bytes:([0-9]*) ', output)[0])
    tx_bytes = int(re.findall('TX bytes:([0-9]*) ', output)[0])

    return (rx_bytes, tx_bytes)

class NetError(Exception):
    pass