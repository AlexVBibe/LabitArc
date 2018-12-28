using Labit.Composition;
using Labit.Interfaces;
using Microsoft.Practices.Unity;
using Prism.Commands;
using ProductBacklog.Server.Interfaces;
using System.Windows.Input;

namespace ProductBacklog.Server.ViewModel
{
    [View("ServerDiagnosticMonitorView")]
    class ServerDiagnosticMonitor : BaseViewModel, IServerDiagnosticMonitor, ILoadable
    {
        private bool isServerOnline;
        private bool isClientConnected;
        private bool isDiscoveryOnline;
        private bool isParing;

        [Dependency]
        public IGadgetServer GadgetServer { get; set; }

        [Dependency]
        public IDiscoveryServer DiscoveryServer { get; set; }

        [Dependency]
        public IStateController StateController { get; set; }

        [Dependency]
        public IMouseMonitor MouseMonitor { get; set; }

        public ICommand StartServer { get; set; }
        public ICommand SendMessage { get; set; }

        public ServerDiagnosticMonitor()
        {
            StartServer = new DelegateCommand<object>(ExecuteStartServer);
            SendMessage = new DelegateCommand<object>(ExecuteSendMessage);
        }

        public bool IsServerOnline
        {
            get { return isServerOnline; }
            set { SetField(ref isServerOnline, value, nameof(IsServerOnline)); }
        }

        public bool IsClientConnected
        {
            get { return isClientConnected; }
            set { SetField(ref isClientConnected, value, nameof(IsClientConnected)); }
        }

        public bool IsDiscoveryOnline
        {
            get { return isDiscoveryOnline; }
            set { SetField(ref isDiscoveryOnline, value, nameof(IsDiscoveryOnline)); }
        }

        public bool IsParing
        {
            get { return isParing; }
            set { SetField(ref isParing, value, nameof(IsParing)); }
        }

        public string Message { get; set; }

        private void ExecuteStartServer(object args)
        {
            GadgetServer.Launch();
        }

        private void ExecuteSendMessage(object args)
        {
            GadgetServer.SendMessage(Message);
        }

        public void OnLoaded()
        {
            StateController.Attach(this);
        }

        public void OnUnloaded()
        {
        }
    }
}
