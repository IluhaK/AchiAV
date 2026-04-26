using SafeGuardXUltimate.Models;
using SafeGuardXUltimate.Services;
using System.Collections.ObjectModel;

namespace SafeGuardXUltimate.ViewModels;

public sealed class ExtraFeaturesViewModel : ObservableObject
{
    private bool _gameModeEnabled = true;
    private bool _silentModeEnabled;
    private bool _vpnEnabled;
    private int _battlePassLevel = 7;

    public ExtraFeaturesViewModel(MockDataService mockDataService)
    {
        Achievements = new ObservableCollection<Achievement>(mockDataService.GetAchievements());
        News = new ObservableCollection<NewsItem>(mockDataService.GetNews());
        Features = new[]
        {
            "Daily rewards", "Battle pass demo", "Fake VPN module", "Game mode", "Silent mode",
            "Threat map", "Security news", "Performance monitor", "Rank system", "Animated notifications"
        };
    }

    public ObservableCollection<Achievement> Achievements { get; }
    public ObservableCollection<NewsItem> News { get; }
    public string[] Features { get; }

    public bool GameModeEnabled { get => _gameModeEnabled; set => SetProperty(ref _gameModeEnabled, value); }
    public bool SilentModeEnabled { get => _silentModeEnabled; set => SetProperty(ref _silentModeEnabled, value); }
    public bool VpnEnabled { get => _vpnEnabled; set => SetProperty(ref _vpnEnabled, value); }
    public int BattlePassLevel { get => _battlePassLevel; set => SetProperty(ref _battlePassLevel, value); }
}
