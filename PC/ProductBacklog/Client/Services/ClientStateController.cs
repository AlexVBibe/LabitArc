using Microsoft.Practices.Unity;
using ProductBacklog.Client.Interfaces;
using ProductBacklog.Main;

namespace ProductBacklog.Client.Services
{
    class ClientStateController : IClientStateController
    {
        private readonly IClientDiscoveryService discoveryService;
        private readonly IClientService clientService;
        private IClientDiagnosticMonitor monitor;

        public ClientStateController([Dependency]IClientDiscoveryService discoveryService, [Dependency]IClientService clientService)
        {
            this.discoveryService = discoveryService;
            this.clientService = clientService;
        }

        public void Attach(IClientDiagnosticMonitor monitor)
        {
            this.monitor = monitor;
            clientService.OnClientStateChanged += ClientStateChanged;
            discoveryService.OnClientDiscoveryStateChanged += ClientDiscoveryStateChanged;

            ClientStateChanged(clientService.Status);
            ClientDiscoveryStateChanged(discoveryService.Status);
        }

        private void ClientDiscoveryStateChanged(DiscoveryStatus status)
        {
            System.Diagnostics.Debug.WriteLine(status);

            switch (status)
            {
                case DiscoveryStatus.None:
                    break;
                case DiscoveryStatus.Online:
                    monitor.IsServerOnline = true;
                    break;
                case DiscoveryStatus.Pairing:
                    monitor.IsParing = true;
                    break;
                case DiscoveryStatus.Paired:
                    System.Diagnostics.Debug.WriteLine(discoveryService.HostAddress.Address);
                    discoveryService.Shutdown();
                    clientService.HostAddress = discoveryService.HostAddress;
                    clientService.Launch();
                    break;
            }
        }

        private void ClientStateChanged(ClientStatus status)
        {
            switch(status)
            {
                case ClientStatus.Connected:
                    monitor.IsParing = false;
                    monitor.IsServerOnline = true;
                    discoveryService.Shutdown();
                    break;
                case ClientStatus.Disconnected:
                    monitor.IsServerOnline = false;
                    discoveryService.Launch();
                    break;
            }
        }
    }
}
