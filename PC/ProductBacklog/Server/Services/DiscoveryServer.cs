using ProductBacklog.Extensions;
using ProductBacklog.Main;
using ProductBacklog.Server.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ProductBacklog.Server.Services
{
    class DiscoveryServer : IDiscoveryServer
    {
        public event DiscoveryStateChangedDelegate OnDiscoveryStateChanged;

        public DiscoveryStatus Status { get; set; }

        private Socket socket;

        public void Shutdown()
        {
            if (socket != null)
            {
                socket.Close();
                socket = null;
            }
        }

        public void Launch()
        {
            if (socket != null)
                return;

            Task.Run(() =>
            {
                byte[] data = new byte[1024];
                var endPoint = NetworkUtils.LocalIPAddress();
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                var iep = new IPEndPoint(endPoint, Constants.DISCOVERYPORT);
                socket.Bind(iep);
                UpdateStatus(DiscoveryStatus.Online);

                EndPoint ep = (EndPoint)iep;
                try
                {
                    while (Status != DiscoveryStatus.None)
                    {
                        var recv = socket.ReceiveFrom(data, ref ep);
                        if (recv > 0)
                            ProcessReceivedBytes(data, recv);
                    }
                }
                catch (Exception ex)
                {
                }
            })
            .ContinueWith(t =>
            {
                try
                {
                    var ex = t.Exception;
                }
                catch(Exception ex)
                {
                }

                UpdateStatus(DiscoveryStatus.None);
                socket?.Close();
                socket = null;
            });
        }

        private void ProcessReceivedBytes(byte[] dataBytes, int size)
        {
            var command = Encoding.ASCII.GetString(dataBytes, 0, size).ToUpper();
            if (command == Constants.WHERE_ARE_YOU)
            {
                UpdateStatus(DiscoveryStatus.Pairing);

                var endPoint = NetworkUtils.LocalIPAddress();
                var message = $"{endPoint}:{Constants.DATAPORT}";
                SendMessage(message);
            }
        }

        private void SendMessage(string message)
        {
            Task.Run(() =>
            {
                var client = new UdpClient();
                var endPoint = NetworkUtils.LocalIPAddress();
                var ip = new IPEndPoint(endPoint, Constants.DATAPORT);
                var data = Encoding.ASCII.GetBytes(message);
                client.Send(data, data.Length, ip);
                client.Close();
            });
        }

        private void UpdateStatus(DiscoveryStatus newStatus)
        {
            Status = newStatus;
            OnDiscoveryStateChanged?.Invoke(Status);
        }
    }
}
