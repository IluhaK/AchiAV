using SafeGuardXUltimate.ViewModels;
using System.Windows;

namespace SafeGuardXUltimate;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }
}
