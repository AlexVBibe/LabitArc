namespace ProductBacklog.Client.Interfaces
{
    interface IClientStateController
    {
        void Attach(IClientDiagnosticMonitor monitor);
    }
}
