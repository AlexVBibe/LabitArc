using ProductBacklog.Main;

namespace ProductBacklog.Client.Interfaces
{
    public delegate void MessageHandler(string message);

    public enum ClientStatus
    {
        Connected,      // Client connected; Server has a client connection
        Disconnected    // Client is disconnected, Server lost client
    }

    public delegate void ClientStateChangedDelegate(ClientStatus status);

    interface IClientService
    {
        event ClientStateChangedDelegate OnClientStateChanged;
        event MessageHandler OnMessageHandler;

        ClientStatus Status { get; set; }
        HostAddress HostAddress { get; set; }
        void Launch();
        void ShutDown();
        void SendMessage(string message);
    }
}
