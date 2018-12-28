using ProductBacklog.Server.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProductBacklog.Server.View
{
    /// <summary>
    /// Interaction logic for MouseMonitorView.xaml
    /// </summary>
    public partial class MouseMonitorView : UserControl
    {
        public MouseMonitorView()
        {
            InitializeComponent();

            this.MouseMove += MouseMonitorView_MouseMove;
            this.MouseDown += MouseMonitorView_MouseDown;
        }

        private void MouseMonitorView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var monitor = this.DataContext as IMouseMonitor;
            if (monitor != null && monitor.IsEnabled)
            {
                monitor.MouseData(new Point(0, 0));
            }
        }

        private void MouseMonitorView_MouseMove(object sender, MouseEventArgs e)
        {
            var monitor = this.DataContext as IMouseMonitor;
            if (monitor != null && monitor.IsEnabled)
            {
                monitor.MouseData(e.GetPosition(this));
            }
        }
    }
}
