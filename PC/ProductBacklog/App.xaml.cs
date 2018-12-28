using Labit.Composition;
using Labit.Interfaces;
using Microsoft.Practices.Unity;
using ProductBacklog.Client.Interfaces;
using ProductBacklog.Client.Services;
using ProductBacklog.Client.ViewModel;
using ProductBacklog.Main;
using ProductBacklog.Server.Interfaces;
using ProductBacklog.Server.Services;
using ProductBacklog.Server.ViewModel;
using System.Windows;

namespace Labit
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IUnityContainer GlobalContainer;

        protected override void OnExit(ExitEventArgs e)
        {
            var registry = GlobalContainer.Resolve<ICompositeViewRegistry>();
            var views = registry.Views;
            foreach (var view in views)
            {
                if ((view as CompositeView).DataContext is ILoadable)
                {
                    var loadable = (view as CompositeView).DataContext as ILoadable;
                    loadable.OnUnloaded();
                }
            }
            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            App.GlobalContainer = new UnityContainer();

            // infrastructure bits and bobs
            GlobalContainer.RegisterType<ICompositeViewRegistry, CompositeViewRegistry>(new ContainerControlledLifetimeManager());
            GlobalContainer.RegisterType<IShellViewModel, ShellViewModel>(new ContainerControlledLifetimeManager());
            GlobalContainer.RegisterType<IHostWindowViewModel, HostWindowViewModel>(new ContainerControlledLifetimeManager());

            //// product parts
            GlobalContainer.RegisterType<IClientDiagnosticMonitor, ClientDiagnosticMonitor>(new ContainerControlledLifetimeManager());
            GlobalContainer.RegisterType<IServerDiagnosticMonitor, ServerDiagnosticMonitor>(new ContainerControlledLifetimeManager());

            GlobalContainer.RegisterType<IGadgetServer, GadgetServer>(new ContainerControlledLifetimeManager());
            GlobalContainer.RegisterType<IDiscoveryServer, DiscoveryServer>(new ContainerControlledLifetimeManager());
            GlobalContainer.RegisterType<IStateController, StateController>(new ContainerControlledLifetimeManager());
            GlobalContainer.RegisterType<IMouseMonitor, MouseMonitor>(new ContainerControlledLifetimeManager());

            GlobalContainer.RegisterType<IClientStateController, ClientStateController>(new ContainerControlledLifetimeManager());
            GlobalContainer.RegisterType<IClientDiscoveryService, ClientDiscoveryService>(new ContainerControlledLifetimeManager());
            GlobalContainer.RegisterType<IClientService, ClientService>(new ContainerControlledLifetimeManager());

            GlobalContainer.RegisterType<IVelocityCalculationService, VelocityCalculationService>(new ContainerControlledLifetimeManager());
            GlobalContainer.RegisterType<IDistanceCalculationService, DistanceCalculationService>(new ContainerControlledLifetimeManager());

            base.OnStartup(e);
        }

        public static T Resolve<T>() where T : class
        {
            return GlobalContainer.Resolve<T>();
        }

        public static T CreateDialog<T>() where T : class
        {
            var window = new HostWindow();
            var result = GlobalContainer.Resolve<IHostWindowViewModel>();
            result.Content = GlobalContainer.Resolve<T>();
            window.DataContext = result;
            window.Owner = App.Current.MainWindow;
            window.ShowDialog();
            return result.Content as T;
        }

        public static bool? CreateDialog<T>(T viewModel) where T : class
        {
            var window = new HostWindow();
            var hostViewModel = GlobalContainer.Resolve<IHostWindowViewModel>();
            hostViewModel.Content = viewModel;
            window.DataContext = hostViewModel;
            window.Owner = App.Current.MainWindow;

            if (viewModel is IClosable)
                (viewModel as IClosable).OnClose = (result) =>
                {
                    window.DialogResult = result;
                    window.Close();
                };
            return window.ShowDialog();
        }
    }
}
