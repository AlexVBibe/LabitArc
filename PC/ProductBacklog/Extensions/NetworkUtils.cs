using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace ProductBacklog.Extensions
{
    public static class NetworkUtils
    {
        public static IPAddress LocalIPAddress()
        {
#if _DEBUG
            return IPAddress.Loopback;
#endif
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            var host = Dns.GetHostEntry(Dns.GetHostName());

            return host?.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }

        public static string MakeBroadcastAddress(string localAddress)
        {
            var parts = localAddress.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            parts[parts.Length - 1] = "255";
            return string.Join(".", parts);
        }
    }
}
