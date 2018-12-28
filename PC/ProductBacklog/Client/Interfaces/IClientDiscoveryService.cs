using ProductBacklog.Main;

namespace ProductBacklog.Client.Interfaces
{
    public delegate void ClientDiscoveryStateChangedDelegate(DiscoveryStatus status);

    interface IClientDiscoveryService
    {
        event ClientDiscoveryStateChangedDelegate OnClientDiscoveryStateChanged;

        DiscoveryStatus Status { get; }
        HostAddress HostAddress { get; }
        void Shutdown();
        void Launch();
    }
}
