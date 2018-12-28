namespace ProductBacklog.Server.Interfaces
{
    interface IStateController
    {
        void Attach(IServerDiagnosticMonitor monitor);
    }
}
