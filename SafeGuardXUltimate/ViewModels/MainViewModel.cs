using SafeGuardXUltimate.Commands;
using SafeGuardXUltimate.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SafeGuardXUltimate.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private object? _currentViewModel;

    public MainViewModel()
    {
        var scan = new ScanSimulationService();
        var mock = new MockDataService();

        Dashboard = new DashboardViewModel(scan, new ActivityFeedService());
        ThreatHunter = new ThreatHunterViewModel(mock, new RewardService());
        ExtraFeatures = new ExtraFeaturesViewModel(mock);

        Modules = new ObservableCollection<ModuleViewModelBase>(new ModuleViewModelBase[]
        {
            new AntivirusViewModel(),
            new SmartScanViewModel(),
            new DeepScanViewModel(),
            new FirewallCenterViewModel(),
            new WebProtectionViewModel(),
            new PrivacyGuardViewModel(),
            new RansomwareShieldViewModel(),
            new SafeBankingViewModel(),
            new WiFiInspectorViewModel(),
            new USBMonitorViewModel(),
            new VulnerabilityScannerViewModel(),
            new PasswordVaultDemoViewModel(),
            new SystemBoosterViewModel(),
            new ProcessMonitorViewModel(),
            new AISecurityAssistantViewModel()
        });

        NavigateDashboardCommand = new RelayCommand(_ => CurrentViewModel = Dashboard);
        NavigateThreatHunterCommand = new RelayCommand(_ => CurrentViewModel = ThreatHunter);
        NavigateExtrasCommand = new RelayCommand(_ => CurrentViewModel = ExtraFeatures);
        NavigateModuleCommand = new RelayCommand(vm => CurrentViewModel = vm);

        CurrentViewModel = Dashboard;
    }

    public DashboardViewModel Dashboard { get; }
    public ThreatHunterViewModel ThreatHunter { get; }
    public ExtraFeaturesViewModel ExtraFeatures { get; }
    public ObservableCollection<ModuleViewModelBase> Modules { get; }

    public object? CurrentViewModel
    {
        get => _currentViewModel;
        set => SetProperty(ref _currentViewModel, value);
    }

    public ICommand NavigateDashboardCommand { get; }
    public ICommand NavigateThreatHunterCommand { get; }
    public ICommand NavigateExtrasCommand { get; }
    public ICommand NavigateModuleCommand { get; }
}
