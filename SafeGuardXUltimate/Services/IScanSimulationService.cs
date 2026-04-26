using System;
using System.Threading;
using System.Threading.Tasks;

namespace SafeGuardXUltimate.Services;

public interface IScanSimulationService
{
    Task RunScanAsync(string mode, Action<int, string> progressCallback, Action<string> logCallback, CancellationToken cancellationToken);
}
