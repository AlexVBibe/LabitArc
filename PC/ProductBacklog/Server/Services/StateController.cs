using Microsoft.Practices.Unity;
using ProductBacklog.Main;
using ProductBacklog.Server.Interfaces;

namespace ProductBacklog.Server.Services
{
    class StateController : IStateController
    {
        private readonly IGadgetServer gadgetServer;
        private readonly IDiscoveryServer discoveryServer;
        private IServerDiagnosticMonitor monitor;

        public StateController([Dependency]IGadgetServer gadgetServer, [Dependency]IDiscoveryServer discoveryServer)
        {
            this.gadgetServer = gadgetServer;
            this.discoveryServer = discoveryServer;
            this.gadgetServer.OnGadgetStateChanged += GadgetStateChanged;
            this.discoveryServer.OnDiscoveryStateChanged += DiscoveryStateChanged;
        }

        public void Attach(IServerDiagnosticMonitor monitor)
        {
            this.monitor = monitor;
            GadgetStateChanged(gadgetServer.Status);
            DiscoveryStateChanged(discoveryServer.Status);
        }

        private void DiscoveryStateChanged(DiscoveryStatus status)
        {
            switch(status)
            {
                case DiscoveryStatus.None:
                    monitor.IsDiscoveryOnline = false;
                    monitor.IsParing = false;
                    break;
                case DiscoveryStatus.Online:
                    monitor.IsDiscoveryOnline = true;
                    break;
                case DiscoveryStatus.Paired:
                    break;
                case DiscoveryStatus.Pairing:
                    monitor.IsParing = true;
                    break;
            }
        }

        private void GadgetStateChanged(ServiceStatus status)
        {
            switch(status)
            {
                case ServiceStatus.Available:
                    monitor.IsServerOnline = true;
                    break;
                case ServiceStatus.Connected:
                    monitor.IsClientConnected = true;
                    discoveryServer.Shutdown();
                    break;
                case ServiceStatus.Disconnected:
                    monitor.IsClientConnected = false;
                    break;
                case ServiceStatus.None:
                    monitor.IsServerOnline = false;
                    break;
            }

            if (monitor.IsServerOnline && !monitor.IsClientConnected)
            {
                discoveryServer.Launch();
            }
        }
    }
}
