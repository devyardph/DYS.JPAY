using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace DYS.JPay.Server.Helpers
{
    public static class NetworkHelper
    {
        public static string? GetLocalWifiIp()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var ni in interfaces!)
            {
                // Only consider interfaces that are up and not loopback
                if (ni.OperationalStatus == OperationalStatus.Up &
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                     ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet))
                {
                    var ipProps = ni.GetIPProperties();
                    foreach (var addr in ipProps.UnicastAddresses)
                    {
                        if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return addr.Address.ToString(); // IPv4 address
                        }
                    }
                }
            }
            return null;
        }

    }
}
