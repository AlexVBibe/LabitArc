namespace ProductBacklog.Client.Interfaces
{
    public interface IClientDiagnosticMonitor
    {
        bool IsServerOnline { get; set; }

        bool IsParing { get; set; }
    }
}
