using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductBacklog.Server.Interfaces
{
    public interface IServerDiagnosticMonitor
    {
        bool IsServerOnline { get; set; }
        bool IsClientConnected { get; set; }
        bool IsDiscoveryOnline { get; set; }
        bool IsParing { get; set; }
    }
}
