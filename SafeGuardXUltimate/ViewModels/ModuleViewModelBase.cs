using SafeGuardXUltimate.Commands;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SafeGuardXUltimate.ViewModels;

public abstract class ModuleViewModelBase : ObservableObject
{
    private bool _isEnabled = true;
    private int _sensitivity = 75;
    private string _status = "Ready";
    private bool _isAnimating;

    protected ModuleViewModelBase(string title, string description)
    {
        Title = title;
        Description = description;
        SimulateCommand = new AsyncRelayCommand(SimulateAsync);
    }

    public string Title { get; }
    public string Description { get; }
    public bool IsEnabled { get => _isEnabled; set => SetProperty(ref _isEnabled, value); }
    public int Sensitivity { get => _sensitivity; set => SetProperty(ref _sensitivity, value); }
    public string Status { get => _status; set => SetProperty(ref _status, value); }
    public bool IsAnimating { get => _isAnimating; set => SetProperty(ref _isAnimating, value); }
    public ICommand SimulateCommand { get; }

    protected virtual async Task SimulateAsync()
    {
        IsAnimating = true;
        Status = "Simulation in progress...";
        await Task.Delay(1200);
        Status = $"{Title}: protection matrix recalibrated at {DateTime.Now:T}";
        IsAnimating = false;
    }
}
