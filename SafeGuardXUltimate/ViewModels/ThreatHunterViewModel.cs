using SafeGuardXUltimate.Commands;
using SafeGuardXUltimate.Models;
using SafeGuardXUltimate.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SafeGuardXUltimate.ViewModels;

public sealed class ThreatHunterViewModel : ObservableObject
{
    private readonly ThreatHunterService _service = new();
    private int _tokens = 100;
    private int _threatsNeutralized;
    private int _drones;
    private int _aiDefenders;
    private string _rank = "Cadet";

    public ThreatHunterViewModel(MockDataService dataService)
    {
        ShopItems = new ObservableCollection<ShopItem>(dataService.GetShopItems());
        HuntCommand = new RelayCommand(_ => Hunt());
        BuyDroneCommand = new RelayCommand(_ => BuyDrone(), _ => Tokens >= 50);
        BuyAiCommand = new RelayCommand(_ => BuyAi(), _ => Tokens >= 120);
    }

    public int Tokens { get => _tokens; set => SetProperty(ref _tokens, value); }
    public int ThreatsNeutralized { get => _threatsNeutralized; set => SetProperty(ref _threatsNeutralized, value); }
    public int Drones { get => _drones; set => SetProperty(ref _drones, value); }
    public int AiDefenders { get => _aiDefenders; set => SetProperty(ref _aiDefenders, value); }
    public string Rank { get => _rank; set => SetProperty(ref _rank, value); }

    public ObservableCollection<ShopItem> ShopItems { get; }
    public ICommand HuntCommand { get; }
    public ICommand BuyDroneCommand { get; }
    public ICommand BuyAiCommand { get; }

    private void Hunt()
    {
        ThreatsNeutralized += 1;
        Tokens += 10 + _service.GetPassiveIncome(Drones, AiDefenders);
        Rank = ThreatsNeutralized switch
        {
            > 250 => "Sentinel X",
            > 120 => "Elite Hunter",
            > 50 => "Defender",
            _ => "Cadet"
        };
    }

    private void BuyDrone() { Tokens -= 50; Drones += 1; }
    private void BuyAi() { Tokens -= 120; AiDefenders += 1; }
}
