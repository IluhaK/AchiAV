using SafeGuardXUltimate.Commands;
using SafeGuardXUltimate.Models;
using SafeGuardXUltimate.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SafeGuardXUltimate.ViewModels;

public sealed class ThreatHunterViewModel : ObservableObject
{
    private readonly ThreatHunterService _service = new();
    private readonly RewardService _rewardService;

    private int _tokens = 100;
    private int _threatsNeutralized;
    private int _drones;
    private int _aiDefenders;
    private int _prestigeLevel;
    private int _bossLevel = 1;
    private int _loginStreak = 3;
    private string _rank = "Cadet";

    public ThreatHunterViewModel(MockDataService dataService, RewardService rewardService)
    {
        _rewardService = rewardService;
        ShopItems = new ObservableCollection<ShopItem>(dataService.GetShopItems());

        Upgrades = new ObservableCollection<UpgradeItem>
        {
            new() { Name = "Scan Drone", Description = "Passive token scouting.", BaseCost = 50 },
            new() { Name = "AI Defender", Description = "Predictive neutralization.", BaseCost = 120 },
            new() { Name = "Quantum Shield", Description = "Multiplier stability.", BaseCost = 240 },
            new() { Name = "Firewall Cannons", Description = "Boss critical damage.", BaseCost = 420 },
            new() { Name = "Auto Neutralizer", Description = "Autoclick and passive combo.", BaseCost = 700 }
        };

        HuntCommand = new RelayCommand(_ => Hunt());
        BuyDroneCommand = new RelayCommand(_ => BuyUpgrade("Scan Drone"), _ => Tokens >= GetUpgradeCost("Scan Drone"));
        BuyAiCommand = new RelayCommand(_ => BuyUpgrade("AI Defender"), _ => Tokens >= GetUpgradeCost("AI Defender"));
        BuyQuantumCommand = new RelayCommand(_ => BuyUpgrade("Quantum Shield"), _ => Tokens >= GetUpgradeCost("Quantum Shield"));
        FightBossCommand = new RelayCommand(_ => FightBoss());
        ClaimDailyRewardCommand = new RelayCommand(_ => ClaimDailyReward());
        PrestigeCommand = new RelayCommand(_ => Prestige(), _ => ThreatsNeutralized >= 200);
    }

    public int Tokens { get => _tokens; set => SetProperty(ref _tokens, value); }
    public int ThreatsNeutralized { get => _threatsNeutralized; set => SetProperty(ref _threatsNeutralized, value); }
    public int Drones { get => _drones; set => SetProperty(ref _drones, value); }
    public int AiDefenders { get => _aiDefenders; set => SetProperty(ref _aiDefenders, value); }
    public int PrestigeLevel { get => _prestigeLevel; set => SetProperty(ref _prestigeLevel, value); }
    public int BossLevel { get => _bossLevel; set => SetProperty(ref _bossLevel, value); }
    public string Rank { get => _rank; set => SetProperty(ref _rank, value); }

    public ObservableCollection<ShopItem> ShopItems { get; }
    public ObservableCollection<UpgradeItem> Upgrades { get; }

    public ICommand HuntCommand { get; }
    public ICommand BuyDroneCommand { get; }
    public ICommand BuyAiCommand { get; }
    public ICommand BuyQuantumCommand { get; }
    public ICommand FightBossCommand { get; }
    public ICommand ClaimDailyRewardCommand { get; }
    public ICommand PrestigeCommand { get; }

    private void Hunt()
    {
        ThreatsNeutralized += 1;
        var multiplier = _rewardService.GetPrestigeMultiplier(PrestigeLevel);
        Tokens += (10 + _service.GetPassiveIncome(Drones, AiDefenders)) * multiplier;

        Rank = ThreatsNeutralized switch
        {
            > 500 => "Omega Sentinel",
            > 250 => "Sentinel X",
            > 120 => "Elite Hunter",
            > 50 => "Defender",
            _ => "Cadet"
        };
    }

    private void FightBoss()
    {
        var reward = _rewardService.GetBossReward(BossLevel);
        Tokens += reward;
        ThreatsNeutralized += 5 + BossLevel;
        BossLevel += 1;
    }

    private void ClaimDailyReward()
    {
        Tokens += _rewardService.GetDailyReward(_loginStreak);
        _loginStreak += 1;
    }

    private void Prestige()
    {
        PrestigeLevel += 1;
        ThreatsNeutralized = 0;
        Drones = 0;
        AiDefenders = 0;
        Tokens = 100 + 50 * PrestigeLevel;
    }

    private void BuyUpgrade(string name)
    {
        var upgrade = Upgrades.First(u => u.Name == name);
        var cost = GetUpgradeCost(name);
        if (Tokens < cost) return;

        Tokens -= cost;
        upgrade.Level += 1;

        if (name == "Scan Drone") Drones += 1;
        if (name == "AI Defender") AiDefenders += 1;

        RaisePropertyChanged(nameof(Upgrades));
        RaisePropertyChanged(nameof(Tokens));
    }

    private int GetUpgradeCost(string name)
    {
        var upgrade = Upgrades.First(u => u.Name == name);
        return upgrade.BaseCost + (upgrade.Level * 25);
    }

}
