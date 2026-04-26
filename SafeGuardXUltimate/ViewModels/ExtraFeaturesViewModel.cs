using SafeGuardXUltimate.Models;
using SafeGuardXUltimate.Services;
using System.Collections.ObjectModel;

namespace SafeGuardXUltimate.ViewModels;

public sealed class ExtraFeaturesViewModel : ObservableObject
{
    public ExtraFeaturesViewModel(MockDataService mockDataService)
    {
        Achievements = new ObservableCollection<Achievement>(mockDataService.GetAchievements());
        News = new ObservableCollection<NewsItem>(mockDataService.GetNews());
        Features =
        [
            "Daily rewards", "Battle pass demo", "Fake VPN module", "Game mode", "Silent mode",
            "Threat map", "Security news", "Performance monitor", "Rank system", "Animated notifications"
        ];
    }

    public ObservableCollection<Achievement> Achievements { get; }
    public ObservableCollection<NewsItem> News { get; }
    public string[] Features { get; }
}
