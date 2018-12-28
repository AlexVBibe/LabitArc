using ProductBacklog.Main;

namespace ProductBacklog.Server.Interfaces
{
    public enum ServiceStatus
    {
        None,           // common to all services status set when servcie just created
        Available,      // Server is Available
        Connected,      // Client connected; Server has a client connection
        Disconnected    // Client is disconnected, Server lost client
    }

    public delegate void GadgetStateChangedDelegate(ServiceStatus status);

    interface IGadgetServer
    {
        event GadgetStateChangedDelegate OnGadgetStateChanged;

        void Launch();

        void SendMessage(string message);

        ServiceStatus Status { get; }

        HostAddress HostAddress { get; }
    }
}
