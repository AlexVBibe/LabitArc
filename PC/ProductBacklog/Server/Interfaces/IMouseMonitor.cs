using System.Windows;

namespace ProductBacklog.Server.Interfaces
{
    interface IMouseMonitor
    {
        bool IsEnabled { get; }

        void MouseData(Point pt);
    }
}
