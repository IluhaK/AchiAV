using SafeGuardXUltimate.Commands;
using SafeGuardXUltimate.Models;
using SafeGuardXUltimate.Services;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Input;

namespace SafeGuardXUltimate.ViewModels;

public sealed class DashboardViewModel : ObservableObject
{
    private readonly IScanSimulationService _scanService;
    private CancellationTokenSource? _scanCts;
    private int _progress;
    private string _currentTarget = "Idle";
    private bool _criticalEffectsEnabled;
    private int _securityScore = 97;

    public DashboardViewModel(IScanSimulationService scanService)
    {
        _scanService = scanService;
        ScanLog = new ObservableCollection<ScanLogEntry>();
        Threats = new ObservableCollection<ThreatItem>();
        QuickScanCommand = new AsyncRelayCommand(() => RunScanAsync("Quick"));
        FullScanCommand = new AsyncRelayCommand(() => RunScanAsync("Full"));
        DeepScanCommand = new AsyncRelayCommand(() => RunScanAsync("Deep", enableEffects: true));
    }

    public int SecurityScore { get => _securityScore; set => SetProperty(ref _securityScore, value); }
    public int Progress { get => _progress; set => SetProperty(ref _progress, value); }
    public string CurrentTarget { get => _currentTarget; set => SetProperty(ref _currentTarget, value); }
    public bool CriticalEffectsEnabled { get => _criticalEffectsEnabled; set => SetProperty(ref _criticalEffectsEnabled, value); }

    public ObservableCollection<ScanLogEntry> ScanLog { get; }
    public ObservableCollection<ThreatItem> Threats { get; }

    public ICommand QuickScanCommand { get; }
    public ICommand FullScanCommand { get; }
    public ICommand DeepScanCommand { get; }

    private async System.Threading.Tasks.Task RunScanAsync(string mode, bool enableEffects = false)
    {
        _scanCts?.Cancel();
        _scanCts = new CancellationTokenSource();
        ScanLog.Clear();
        Threats.Clear();
        Progress = 0;

        if (enableEffects)
        {
            CriticalEffectsEnabled = true;
            _ = System.Threading.Tasks.Task.Run(async () =>
            {
                await System.Threading.Tasks.Task.Delay(12000);
                CriticalEffectsEnabled = false;
            });
        }

        await _scanService.RunScanAsync(mode,
            (value, target) => { Progress = value; CurrentTarget = target; },
            message =>
            {
                ScanLog.Add(new ScanLogEntry { Message = message });
                if (message.Contains("Threat detected"))
                {
                    Threats.Add(new ThreatItem { Name = message.Split(':')[1].Split('-')[0].Trim(), Severity = "High", Path = CurrentTarget, IsNeutralized = true });
                }
            },
            _scanCts.Token);

        SecurityScore = Threats.Count == 0 ? 99 : 93;
        CriticalEffectsEnabled = false;
    }
}
