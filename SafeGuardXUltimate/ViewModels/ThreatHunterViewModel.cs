using SafeGuardXUltimate.Commands;
using SafeGuardXUltimate.Models;
using SafeGuardXUltimate.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace SafeGuardXUltimate.ViewModels;

/// <summary>
/// Simple built-in clicker module with tokens only (non-game arcade style).
/// </summary>
public sealed class ThreatHunterViewModel : ObservableObject
{
    private readonly DispatcherTimer _autoTimer;

    private int _tokens = 50;
    private int _clickPower = 1;
    private int _autoTokensPerTick;
    private int _multiplier = 1;
    private string _status = "Ready";
    private ShopItem? _selectedShopItem;

    public ThreatHunterViewModel(MockDataService dataService, RewardService rewardService)
    {
        ShopItems = new ObservableCollection<ShopItem>(dataService.GetShopItems());
        OwnedCosmetics = new ObservableCollection<string>();

        ClickThreatCommand = new RelayCommand(_ => ClickThreat());
        UpgradeClickCommand = new RelayCommand(_ => UpgradeClickPower(), _ => Tokens >= GetClickUpgradeCost());
        UpgradeAutoCommand = new RelayCommand(_ => UpgradeAutoMining(), _ => Tokens >= GetAutoUpgradeCost());
        UpgradeMultiplierCommand = new RelayCommand(_ => UpgradeMultiplier(), _ => Tokens >= GetMultiplierUpgradeCost());
        BuyShopItemCommand = new RelayCommand(_ => BuyShopItem(), _ => SelectedShopItem is not null && Tokens >= SelectedShopItem.Cost);

        _autoTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _autoTimer.Tick += (_, _) =>
        {
            if (AutoTokensPerTick <= 0) return;
            Tokens += AutoTokensPerTick * Multiplier;
            Status = $"Auto token mining: +{AutoTokensPerTick * Multiplier}/s";
        };
        _autoTimer.Start();
    }

    public int Tokens { get => _tokens; set => SetProperty(ref _tokens, value); }
    public int ClickPower { get => _clickPower; set => SetProperty(ref _clickPower, value); }
    public int AutoTokensPerTick { get => _autoTokensPerTick; set => SetProperty(ref _autoTokensPerTick, value); }
    public int Multiplier { get => _multiplier; set => SetProperty(ref _multiplier, value); }
    public string Status { get => _status; set => SetProperty(ref _status, value); }

    public ObservableCollection<ShopItem> ShopItems { get; }
    public ObservableCollection<string> OwnedCosmetics { get; }

    public ShopItem? SelectedShopItem { get => _selectedShopItem; set => SetProperty(ref _selectedShopItem, value); }

    public ICommand ClickThreatCommand { get; }
    public ICommand UpgradeClickCommand { get; }
    public ICommand UpgradeAutoCommand { get; }
    public ICommand UpgradeMultiplierCommand { get; }
    public ICommand BuyShopItemCommand { get; }

    public int GetClickUpgradeCost() => 40 + (ClickPower * 25);
    public int GetAutoUpgradeCost() => 80 + (AutoTokensPerTick * 50);
    public int GetMultiplierUpgradeCost() => 150 + (Multiplier * 120);

    private void ClickThreat()
    {
        Tokens += ClickPower * Multiplier;
        Status = $"Manual neutralization: +{ClickPower * Multiplier} tokens";
    }

    private void UpgradeClickPower()
    {
        var cost = GetClickUpgradeCost();
        if (Tokens < cost) return;
        Tokens -= cost;
        ClickPower += 1;
        Status = $"Click Power upgraded to {ClickPower}";
    }

    private void UpgradeAutoMining()
    {
        var cost = GetAutoUpgradeCost();
        if (Tokens < cost) return;
        Tokens -= cost;
        AutoTokensPerTick += 1;
        Status = $"Auto mining upgraded to {AutoTokensPerTick}/s";
    }

    private void UpgradeMultiplier()
    {
        var cost = GetMultiplierUpgradeCost();
        if (Tokens < cost) return;
        Tokens -= cost;
        Multiplier += 1;
        Status = $"Security multiplier upgraded to x{Multiplier}";
    }

    private void BuyShopItem()
    {
        if (SelectedShopItem is null || Tokens < SelectedShopItem.Cost) return;

        Tokens -= SelectedShopItem.Cost;
        OwnedCosmetics.Add($"{SelectedShopItem.Name} unlocked");
        Status = $"Cosmetic applied: {SelectedShopItem.Name}";
        RaisePropertyChanged(nameof(OwnedCosmetics));
    }
}
