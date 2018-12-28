using Labit.Composition;
using Microsoft.Practices.Unity;
using ProductBacklog.Server.Interfaces;
using System.Windows;

namespace ProductBacklog.Server.ViewModel
{
    [View("MouseMonitorView")]
    class MouseMonitor : BaseViewModel, IMouseMonitor
    {
        private bool isEnabled;

        [Dependency]
        public IGadgetServer GadgetServer { get; set; }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set { SetField(ref isEnabled, value, nameof(IsEnabled)); }
        }

        public void MouseData(Point pt)
        {
            GadgetServer.SendMessage($"X{pt.X}:Y{pt.Y}");
        }
    }
}
