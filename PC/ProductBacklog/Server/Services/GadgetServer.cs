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
    class GadgetServer : IGadgetServer
    {
        // Constants
        private Socket socket;
        private Socket client;

        public event GadgetStateChangedDelegate OnGadgetStateChanged;

        public ServiceStatus Status { get; set; }

        public HostAddress HostAddress { get; set; }

        public void SendMessage(string message)
        {
            if (client?.Connected == true && !string.IsNullOrEmpty(message))
            {
                Task.Run(() => client.Send(Encoding.ASCII.GetBytes(message)));
            }
        }

        public void Launch()
        {
            if (socket != null)
                return;

            Task.Run(() =>
            {
                var bytes = new byte[1024];
                var endPoint = NetworkUtils.LocalIPAddress();
                var port = Constants.DATAPORT;
                var localEndPoint = new IPEndPoint(endPoint, port);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(localEndPoint);
                socket.Listen(10);
                HostAddress = new HostAddress($"{endPoint}:{port}");
                UpdateStatus(ServiceStatus.Available);

                while (socket != null)
                {
                    client = socket.Accept();
                    var message = string.Empty;
                    UpdateStatus(ServiceStatus.Connected);

                    try
                    {
                        while (client != null && client.Connected)
                        {
                            int bytesRec = client.Receive(bytes);
                            if (bytesRec != 0)
                            {
                                ProcessReceivedMessage(bytes, bytesRec);
                            }
                        }
                    }
                    catch(Exception ex)
                    {

                    }
                    UpdateStatus(ServiceStatus.Disconnected);
                }
            });
        }

        private void ProcessReceivedMessage(byte[] messageBytes, int size)
        {
            var messageText = Encoding.ASCII.GetString(messageBytes, 0, size);
            System.Diagnostics.Debug.WriteLine(messageText);

            //TODO: parce commands
            switch (messageText.ToUpper())
            {
                case "HELLO":
                    SendMessage("Android server v.1");
                    break;
                case "PAUSE":
                    System.Diagnostics.Debug.WriteLine(messageText);
                    break;
            }
        }

        private void UpdateStatus(ServiceStatus newStatus)
        {
            Status = newStatus;
            OnGadgetStateChanged?.Invoke(Status);
        }
    }
}
