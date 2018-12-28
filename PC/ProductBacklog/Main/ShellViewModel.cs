using Labit.Composition;
using Microsoft.Practices.Unity;
using ProductBacklog.Client.Interfaces;
using ProductBacklog.Server.Interfaces;

namespace ProductBacklog.Main
{
    [View("ShellView")]
    class ShellViewModel : IShellViewModel
    {
        [Dependency]
        public IClientDiagnosticMonitor Client { get; set; }

        [Dependency]
        public IServerDiagnosticMonitor Server { get; set; }
    }
}
