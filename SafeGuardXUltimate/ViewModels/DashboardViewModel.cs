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
    private string _scanStatus = "Ready";
    private bool _criticalEffectsEnabled;
    private int _securityScore = 97;
    private int _filesScanned;
    private int _processesScanned;
    private int _threatsDetected;
    private int _threatsNeutralized;
    private bool _realTimeProtectionEnabled = true;
    private bool _firewallMonitorEnabled = true;
    private bool _webProtectionEnabled = true;
    private bool _privacyProtectionEnabled = true;
    private bool _updateCenterEnabled = true;
    private string _updateChannel = "Stable";
    private string _lastUpdateStatus = "Database synced 15 minutes ago.";
    private string _lastScanReport = "No scans executed this session.";

    public DashboardViewModel(IScanSimulationService scanService, ActivityFeedService activityFeedService)
    {
        _scanService = scanService;
        ScanLog = new ObservableCollection<ScanLogEntry>();
        Threats = new ObservableCollection<ThreatItem>();
        Quarantine = new ObservableCollection<ThreatItem>();
        Activities = new ObservableCollection<ActivityEvent>(activityFeedService.GetInitialEvents());
        LiveMetrics = new ObservableCollection<int>(Enumerable.Range(0, 22).Select(_ => 35));
        VulnerabilityFindings = new ObservableCollection<string>();
        Notifications = new ObservableCollection<string>();

        QuickScanCommand = new AsyncRelayCommand(() => RunScanAsync("Quick"));
        FullScanCommand = new AsyncRelayCommand(() => RunScanAsync("Full"));
        DeepScanCommand = new AsyncRelayCommand(() => RunScanAsync("Deep", enableEffects: true));
        RunVulnerabilityScanCommand = new AsyncRelayCommand(RunVulnerabilityScanAsync);
        UpdateDefinitionsCommand = new AsyncRelayCommand(UpdateDefinitionsAsync);

        _metricTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(700) };
        _metricTimer.Tick += (_, _) => UpdateRealtimeSimulation();
        _metricTimer.Start();
    }

    public int SecurityScore { get => _securityScore; set => SetProperty(ref _securityScore, value); }
    public int Progress { get => _progress; set => SetProperty(ref _progress, value); }
    public string CurrentTarget { get => _currentTarget; set => SetProperty(ref _currentTarget, value); }
    public string ScanStatus { get => _scanStatus; set => SetProperty(ref _scanStatus, value); }
    public bool CriticalEffectsEnabled { get => _criticalEffectsEnabled; set => SetProperty(ref _criticalEffectsEnabled, value); }

    public int FilesScanned { get => _filesScanned; set => SetProperty(ref _filesScanned, value); }
    public int ProcessesScanned { get => _processesScanned; set => SetProperty(ref _processesScanned, value); }
    public int ThreatsDetected { get => _threatsDetected; set => SetProperty(ref _threatsDetected, value); }
    public int ThreatsNeutralized { get => _threatsNeutralized; set => SetProperty(ref _threatsNeutralized, value); }

    public bool RealTimeProtectionEnabled { get => _realTimeProtectionEnabled; set => SetProperty(ref _realTimeProtectionEnabled, value); }
    public bool FirewallMonitorEnabled { get => _firewallMonitorEnabled; set => SetProperty(ref _firewallMonitorEnabled, value); }
    public bool WebProtectionEnabled { get => _webProtectionEnabled; set => SetProperty(ref _webProtectionEnabled, value); }
    public bool PrivacyProtectionEnabled { get => _privacyProtectionEnabled; set => SetProperty(ref _privacyProtectionEnabled, value); }
    public bool UpdateCenterEnabled { get => _updateCenterEnabled; set => SetProperty(ref _updateCenterEnabled, value); }

    public string UpdateChannel { get => _updateChannel; set => SetProperty(ref _updateChannel, value); }
    public string[] UpdateChannels { get; } = new[] { "Stable", "Early Access", "Enterprise LTS" };

    public string LastUpdateStatus { get => _lastUpdateStatus; set => SetProperty(ref _lastUpdateStatus, value); }
    public string LastScanReport { get => _lastScanReport; set => SetProperty(ref _lastScanReport, value); }

    public ObservableCollection<ScanLogEntry> ScanLog { get; }
    public ObservableCollection<ThreatItem> Threats { get; }
    public ObservableCollection<ThreatItem> Quarantine { get; }
    public ObservableCollection<ActivityEvent> Activities { get; }
    public ObservableCollection<int> LiveMetrics { get; }
    public ObservableCollection<string> VulnerabilityFindings { get; }
    public ObservableCollection<string> Notifications { get; }

    public ICommand QuickScanCommand { get; }
    public ICommand FullScanCommand { get; }
    public ICommand DeepScanCommand { get; }
    public ICommand RunVulnerabilityScanCommand { get; }
    public ICommand UpdateDefinitionsCommand { get; }

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
        ScanStatus = $"{mode} scan started";

        if (enableEffects)
        {
            CriticalEffectsEnabled = true;
            AddNotification("Deep Scan enabled holographic threat mode.");
            _ = System.Threading.Tasks.Task.Run(async () =>
            {
                await System.Threading.Tasks.Task.Delay(12000);
                CriticalEffectsEnabled = false;
            });
        }

        await _scanService.RunScanAsync(mode,
            (value, target) =>
            {
                Progress = value;
                CurrentTarget = target;
                FilesScanned += _random.Next(8, mode == "Deep" ? 45 : 25);
                ProcessesScanned += _random.Next(1, 4);
                ScanStatus = value < 100 ? $"Analyzing {target}" : "Finalizing report";
            },
            message =>
            {
                ScanLog.Add(new ScanLogEntry { Message = message });
                if (message.Contains("Threat detected", StringComparison.OrdinalIgnoreCase))
                {
                    var threatName = message.Split(':')[1].Split('-')[0].Trim();
                    var threat = new ThreatItem
                    {
                        Name = threatName,
                        Severity = ThreatsDetected % 2 == 0 ? "Critical" : "High",
                        Path = CurrentTarget,
                        IsNeutralized = true
                    };

                    ThreatsDetected += 1;
                    ThreatsNeutralized += 1;
                    Threats.Add(threat);
                    Quarantine.Add(threat);
                    AddNotification($"{threat.Name} moved to quarantine.");
                    AddActivity("Threat", $"Threat neutralized: {threat.Name}");
                }
            },
            _scanCts.Token);

        SecurityScore = ThreatsDetected == 0 ? 99 : Math.Max(88, 98 - ThreatsDetected);
        ScanStatus = $"{mode} scan completed";
        LastScanReport = BuildScanReport(mode);
        CriticalEffectsEnabled = false;
    }

    private async System.Threading.Tasks.Task RunVulnerabilityScanAsync()
    {
        VulnerabilityFindings.Clear();
        AddActivity("Vulnerability", "Starting local vulnerability assessment...");

        await System.Threading.Tasks.Task.Delay(600);
        VulnerabilityFindings.Add("OpenSSL library outdated in Demo Browser component (simulated).");
        await System.Threading.Tasks.Task.Delay(600);
        VulnerabilityFindings.Add("Weak startup service permissions detected in Demo Agent task.");
        await System.Threading.Tasks.Task.Delay(600);
        VulnerabilityFindings.Add("Optional OS patch KB-2026-UL-14 not applied (simulated). ");

        AddNotification("Vulnerability scan complete. 3 recommendations available.");
    }

    private async System.Threading.Tasks.Task UpdateDefinitionsAsync()
    {
        if (!UpdateCenterEnabled)
        {
            LastUpdateStatus = "Update Center is disabled by policy.";
            return;
        }

        LastUpdateStatus = "Connecting to update mirror...";
        await System.Threading.Tasks.Task.Delay(700);
        LastUpdateStatus = "Downloading signatures package v26.4.118...";
        await System.Threading.Tasks.Task.Delay(700);
        LastUpdateStatus = $"Definitions updated successfully via {UpdateChannel} channel at {DateTime.Now:T}.";
        AddActivity("Update", LastUpdateStatus);
    }

    private string BuildScanReport(string mode)
    {
        return $"[{DateTime.Now:G}] {mode} Scan Report | Files: {FilesScanned}, Processes: {ProcessesScanned}, " +
               $"Detected: {ThreatsDetected}, Neutralized: {ThreatsNeutralized}, SecurityScore: {SecurityScore}.";
    }

    private void UpdateRealtimeSimulation()
    {
        if (LiveMetrics.Count >= 36)
        {
            LiveMetrics.RemoveAt(0);
        }

        var last = LiveMetrics.LastOrDefault();
        var jitter = _random.Next(-5, 8);
        var protectionBonus = RealTimeProtectionEnabled ? 2 : -4;
        var next = Math.Clamp(last + jitter + protectionBonus, 8, 95);
        LiveMetrics.Add(next);

        if (_random.NextDouble() > 0.92)
        {
            AddActivity("Firewall", "Blocked suspicious outbound test-beacon (simulated). ");
        }

        if (_random.NextDouble() > 0.95 && WebProtectionEnabled)
        {
            AddNotification("Web Protection intercepted unsafe URL pattern.");
        }
    }

    private void AddActivity(string type, string message)
    {
        Activities.Insert(0, new ActivityEvent { EventType = type, Message = message });
        while (Activities.Count > 14)
        {
            Activities.RemoveAt(Activities.Count - 1);
        }
    }

    private void AddNotification(string message)
    {
        Notifications.Insert(0, $"{DateTime.Now:T} • {message}");
        while (Notifications.Count > 12)
        {
            Notifications.RemoveAt(Notifications.Count - 1);
        }
    }
}
