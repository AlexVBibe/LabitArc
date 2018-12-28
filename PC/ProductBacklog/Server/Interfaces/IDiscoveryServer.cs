using ProductBacklog.Main;

namespace ProductBacklog.Server.Interfaces
{
    public delegate void DiscoveryStateChangedDelegate(DiscoveryStatus status);

    interface IDiscoveryServer
    {
        event DiscoveryStateChangedDelegate OnDiscoveryStateChanged;

        void Launch();
        void Shutdown();
        DiscoveryStatus Status { get; }
    }
}
