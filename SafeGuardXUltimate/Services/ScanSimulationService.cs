using SafeGuardXUltimate.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SafeGuardXUltimate.Services;

public sealed class ScanSimulationService : IScanSimulationService
{
    private static readonly string[] ThreatNames = ["Trojan.Mock", "Adware.Sim", "Fake.Injector", "Demo.Payload"];

    public async Task RunScanAsync(string mode, Action<int, string> progressCallback, Action<string> logCallback, CancellationToken cancellationToken)
    {
        var random = new Random();
        for (var i = 1; i <= 100; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var target = $"C:/VirtualSystem/Block_{random.Next(1, 9999)}.bin";
            progressCallback(i, target);
            logCallback($"[{mode}] Scanning {target}");

            if (i % 23 == 0)
            {
                var threat = ThreatNames[random.Next(ThreatNames.Length)];
                logCallback($"Threat detected: {threat} -> neutralized");
            }

            await Task.Delay(mode == "Deep" ? 100 : 50, cancellationToken);
        }

        logCallback($"{mode} scan completed. System status: SECURE");
    }
}
