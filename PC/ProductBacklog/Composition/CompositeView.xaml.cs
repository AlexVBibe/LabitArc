using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Unity;
using Labit.Interfaces;

namespace Labit.Composition
{
    /// <summary>
    /// Interaction logic for CompositeView.xaml
    /// </summary>
    public partial class CompositeView : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel",
            typeof(object),
            typeof(CompositeView),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, null));

        public object ViewModel
        {
            get { return GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public CompositeView()
        {
            InitializeComponent();

            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            var loadable = this.DataContext as ILoadable;
            if (loadable != null)
            {
                loadable.OnUnloaded();
            }
            var registry = App.GlobalContainer.Resolve<ICompositeViewRegistry>();
            registry.UnregisterView(this);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var registry = App.GlobalContainer.Resolve<ICompositeViewRegistry>();
            registry.RegistorView(this);

            var loadable = this.DataContext as ILoadable;
            if (loadable != null)
            {
                loadable.OnLoaded();
            }
        }
    }
}
