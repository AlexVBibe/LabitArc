using System.Windows.Controls;
using ProductBacklog.Client.ViewModel;

namespace ProductBacklog.Client.View
{
    /// <summary>
    /// Interaction logic for ClientDiagnosticMonitorView.xaml
    /// </summary>
    public partial class ClientDiagnosticMonitorView : UserControl
    {
        public ClientDiagnosticMonitorView()
        {
            InitializeComponent();
            Loaded += ClientDiagnosticMonitorView_Loaded;
        }

        private void ClientDiagnosticMonitorView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var vm = this.DataContext as ClientDiagnosticMonitor;
            if (vm != null)
            {
                //vm.Movements(offset =>
                //    {
                //        this.Dispatcher.Invoke(() =>
                //        {
                //            if (Canvas.GetLeft(this.canvas)  + offset < 0)
                //                Canvas.SetLeft(this.canvas, 0);
                //            else if (Canvas.GetLeft(this.canvas) + offset > Canvas.GetRight(parent))
                //                Canvas.SetLeft(this.canvas, Canvas.GetRight(parent)-20);
                //            else
                //                Canvas.SetLeft(this.canvas, Canvas.GetLeft(this.canvas) + offset);
                //        });
                //    });
            }
        }
    }
}
