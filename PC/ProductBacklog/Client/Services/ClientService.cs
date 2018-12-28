using ProductBacklog.Client.Interfaces;
using ProductBacklog.Main;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ProductBacklog.Client.Services
{
    class ClientService : IClientService
    {
        public event ClientStateChangedDelegate OnClientStateChanged;
        public event MessageHandler OnMessageHandler;


        private Socket socket;

        public ClientStatus Status { get; set; }

        public HostAddress HostAddress { get; set; }

        public void ShutDown()
        {
            if (socket != null)
            {
                socket.Close();
                socket = null;
            }
            HostAddress = null;
        }

        public void Launch()
        {
            if (HostAddress == null)
            {
                UpdateStatus(ClientStatus.Disconnected);
                return;
            }

            Task.Run(() =>
            {
                var ipAddress = IPAddress.Parse(HostAddress.Ip);
                var remoteEp = new IPEndPoint(ipAddress, HostAddress.Port);
                socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(remoteEp);

                UpdateStatus(ClientStatus.Connected);

                SendMessage("hello");

                while (socket != null && socket.Connected)
                {
                    try
                    {
                        var bytes = new byte[1024];
                        var bytesRec = socket.Receive(bytes);
                        if (bytesRec == 0)
                            break;
                        var message = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        while (socket.Available > 0)
                        {
                            bytesRec = socket.Receive(bytes);
                            message += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        }

                        OnMessageHandler?.Invoke(message);
                    }
                    catch(Exception ex)
                    {
                        break;
                    }
                }
                UpdateStatus(ClientStatus.Disconnected);
                socket?.Close();
            });
        }

        public void SendMessage(string message)
        {
            if (socket?.Connected == false || string.IsNullOrEmpty(message))
                return;

            Task.Run(() =>
            {
                var msgOut = Encoding.ASCII.GetBytes(message);
                socket.Send(msgOut);
            });
        }

        private void UpdateStatus(ClientStatus newStatus)
        {
            Status = newStatus;
            OnClientStateChanged?.Invoke(Status);
        }
    }
}
