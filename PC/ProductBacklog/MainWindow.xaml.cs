using System.Windows;
using Microsoft.Practices.Unity;
using ProductBacklog.Main;

namespace Labit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = this;
            ViewModel = App.GlobalContainer.Resolve<IShellViewModel>();
            InitializeComponent();
        }

        public IShellViewModel ViewModel { get; set; }
    }
}
