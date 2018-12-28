using System;
using ProductBacklog.Client.Interfaces;
using ProductBacklog.Extensions;
using ProductBacklog.Main;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductBacklog.Client.Services
{
    class ClientDiscoveryService : IClientDiscoveryService
    {
        public event ClientDiscoveryStateChangedDelegate OnClientDiscoveryStateChanged;

        public DiscoveryStatus Status { get; set; }
        public HostAddress HostAddress { get; set; }

        public void Shutdown()
        {
            UpdateStatus(DiscoveryStatus.None);
        }

        public void Launch()
        {
            Task.Run(() =>
            {
                var endPoint = NetworkUtils.LocalIPAddress();
                var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                var iep = new IPEndPoint(endPoint, Constants.DATAPORT);
                sock.Bind(iep);
                UpdateStatus(DiscoveryStatus.Online);

                var ep = (EndPoint)iep;
                var data = new byte[1024];
                while (Status != DiscoveryStatus.None)
                {
                    UpdateStatus(DiscoveryStatus.Pairing);
                    using (Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ => SayHello()))
                    {
                        var recv = sock.ReceiveFrom(data, ref ep);
                        HostAddress = new HostAddress(Encoding.ASCII.GetString(data, 0, recv));
                        UpdateStatus(DiscoveryStatus.Paired);
                    }
                }

                sock.Close();
                UpdateStatus(DiscoveryStatus.None);
            });
        }

        private void SayHello()
        {
            var client = new UdpClient();
            var endPoint = NetworkUtils.LocalIPAddress();
            var b = NetworkUtils.MakeBroadcastAddress(endPoint.ToString());
            endPoint = IPAddress.Parse(b);
            var ip = new IPEndPoint(endPoint, Constants.DISCOVERYPORT);

            var data = Encoding.ASCII.GetBytes(Constants.WHERE_ARE_YOU);
            client.Send(data, data.Length, ip);

            client.Close();
        }

        private void UpdateStatus(DiscoveryStatus newStatus)
        {
            Status = newStatus;
            OnClientDiscoveryStateChanged?.Invoke(Status);
        }
    }
}
