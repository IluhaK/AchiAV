using SafeGuardXUltimate.Commands;
using SafeGuardXUltimate.Models;
using SafeGuardXUltimate.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SafeGuardXUltimate.ViewModels;

public sealed class MainViewModel : ObservableObject
{
    private readonly ThemeService _themeService;
    private object? _currentViewModel;
    private AppThemeOption? _selectedTheme;

    public MainViewModel()
    {
        var scan = new ScanSimulationService();
        var mock = new MockDataService();
        _themeService = new ThemeService();

        Dashboard = new DashboardViewModel(scan, new ActivityFeedService());
        ThreatHunter = new ThreatHunterViewModel(mock, new RewardService());
        ExtraFeatures = new ExtraFeaturesViewModel(mock);

        Modules = new ObservableCollection<ModuleViewModelBase>(new ModuleViewModelBase[]
        {
            new AntivirusViewModel(), new SmartScanViewModel(), new DeepScanViewModel(), new FirewallCenterViewModel(),
            new WebProtectionViewModel(), new PrivacyGuardViewModel(), new RansomwareShieldViewModel(), new SafeBankingViewModel(),
            new WiFiInspectorViewModel(), new USBMonitorViewModel(), new VulnerabilityScannerViewModel(), new PasswordVaultDemoViewModel(),
            new SystemBoosterViewModel(), new ProcessMonitorViewModel(), new AISecurityAssistantViewModel()
        });

        Themes = new ObservableCollection<AppThemeOption>(_themeService.GetThemes());
        SelectedTheme = Themes[0];

        NavigateDashboardCommand = new RelayCommand(_ => CurrentViewModel = Dashboard);
        NavigateThreatHunterCommand = new RelayCommand(_ => CurrentViewModel = ThreatHunter);
        NavigateExtrasCommand = new RelayCommand(_ => CurrentViewModel = ExtraFeatures);
        NavigateModuleCommand = new RelayCommand(vm => CurrentViewModel = vm);
        ChangeThemeCommand = new RelayCommand(_ => ApplySelectedTheme(), _ => SelectedTheme is not null);

        CurrentViewModel = Dashboard;
    }

    public DashboardViewModel Dashboard { get; }
    public ThreatHunterViewModel ThreatHunter { get; }
    public ExtraFeaturesViewModel ExtraFeatures { get; }
    public ObservableCollection<ModuleViewModelBase> Modules { get; }

    public ObservableCollection<AppThemeOption> Themes { get; }
    public AppThemeOption? SelectedTheme
    {
        get => _selectedTheme;
        set => SetProperty(ref _selectedTheme, value);
    }

    public object? CurrentViewModel { get => _currentViewModel; set => SetProperty(ref _currentViewModel, value); }

    public ICommand NavigateDashboardCommand { get; }
    public ICommand NavigateThreatHunterCommand { get; }
    public ICommand NavigateExtrasCommand { get; }
    public ICommand NavigateModuleCommand { get; }
    public ICommand ChangeThemeCommand { get; }

    private void ApplySelectedTheme()
    {
        if (SelectedTheme is null) return;
        _themeService.ApplyTheme(SelectedTheme.ResourcePath);
    }
}
