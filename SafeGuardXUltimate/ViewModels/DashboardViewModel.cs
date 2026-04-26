using SafeGuardXUltimate.Commands;
using SafeGuardXUltimate.Models;
using SafeGuardXUltimate.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;

namespace SafeGuardXUltimate.ViewModels;

public sealed class DashboardViewModel : ObservableObject
{
    private readonly IScanSimulationService _scanService;
    private readonly DispatcherTimer _metricTimer;
    private CancellationTokenSource? _scanCts;
    private readonly Random _random = new();

    private int _progress;
    private string _currentTarget = "Idle";
    private bool _criticalEffectsEnabled;
    private int _securityScore = 97;
    private int _filesScanned;
    private int _processesScanned;
    private int _threatsDetected;
    private int _threatsNeutralized;

    public DashboardViewModel(IScanSimulationService scanService, ActivityFeedService activityFeedService)
    {
        _scanService = scanService;
        ScanLog = new ObservableCollection<ScanLogEntry>();
        Threats = new ObservableCollection<ThreatItem>();
        Activities = new ObservableCollection<ActivityEvent>(activityFeedService.GetInitialEvents());
        LiveMetrics = new ObservableCollection<int>(Enumerable.Range(0, 20).Select(_ => 35));

        QuickScanCommand = new AsyncRelayCommand(() => RunScanAsync("Quick"));
        FullScanCommand = new AsyncRelayCommand(() => RunScanAsync("Full"));
        DeepScanCommand = new AsyncRelayCommand(() => RunScanAsync("Deep", enableEffects: true));

        _metricTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(600) };
        _metricTimer.Tick += (_, _) => UpdateMetrics();
        _metricTimer.Start();
    }

    public int SecurityScore { get => _securityScore; set => SetProperty(ref _securityScore, value); }
    public int Progress { get => _progress; set => SetProperty(ref _progress, value); }
    public string CurrentTarget { get => _currentTarget; set => SetProperty(ref _currentTarget, value); }
    public bool CriticalEffectsEnabled { get => _criticalEffectsEnabled; set => SetProperty(ref _criticalEffectsEnabled, value); }

    public int FilesScanned { get => _filesScanned; set => SetProperty(ref _filesScanned, value); }
    public int ProcessesScanned { get => _processesScanned; set => SetProperty(ref _processesScanned, value); }
    public int ThreatsDetected { get => _threatsDetected; set => SetProperty(ref _threatsDetected, value); }
    public int ThreatsNeutralized { get => _threatsNeutralized; set => SetProperty(ref _threatsNeutralized, value); }

    public ObservableCollection<ScanLogEntry> ScanLog { get; }
    public ObservableCollection<ThreatItem> Threats { get; }
    public ObservableCollection<ActivityEvent> Activities { get; }
    public ObservableCollection<int> LiveMetrics { get; }

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
        FilesScanned = 0;
        ProcessesScanned = 0;
        ThreatsDetected = 0;
        ThreatsNeutralized = 0;

        if (enableEffects)
        {
            CriticalEffectsEnabled = true;
            _ = System.Threading.Tasks.Task.Run(async () =>
            {
                await System.Threading.Tasks.Task.Delay(12000);
                CriticalEffectsEnabled = false;
            });
        }

        await _scanService.RunScanAsync(
            mode,
            (value, target) =>
            {
                Progress = value;
                CurrentTarget = target;
                FilesScanned += _random.Next(8, 30);
                ProcessesScanned += _random.Next(1, 4);
            },
            message =>
            {
                ScanLog.Add(new ScanLogEntry { Message = message });

                if (message.Contains("Threat detected", StringComparison.OrdinalIgnoreCase))
                {
                    ThreatsDetected += 1;
                    ThreatsNeutralized += 1;

                    var threatName = message.Split(':')[1].Split('-')[0].Trim();
                    Threats.Add(new ThreatItem
                    {
                        Name = threatName,
                        Severity = ThreatsDetected % 2 == 0 ? "Critical" : "High",
                        Path = CurrentTarget,
                        IsNeutralized = true
                    });

                    Activities.Insert(0, new ActivityEvent
                    {
                        EventType = "Threat",
                        Message = $"{threatName} neutralized in {CurrentTarget}"
                    });

                    while (Activities.Count > 12)
                    {
                        Activities.RemoveAt(Activities.Count - 1);
                    }
                }
            },
            _scanCts.Token);

        SecurityScore = ThreatsDetected == 0 ? 99 : 94;
        CriticalEffectsEnabled = false;
    }

    private void UpdateMetrics()
    {
        if (LiveMetrics.Count >= 36)
        {
            LiveMetrics.RemoveAt(0);
        }

        var last = LiveMetrics.LastOrDefault();
        var next = Math.Clamp(last + _random.Next(-6, 7), 10, 95);
        LiveMetrics.Add(next);
    }
}
