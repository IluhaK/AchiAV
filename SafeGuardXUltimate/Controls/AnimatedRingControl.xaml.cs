using System.Windows;
using System.Windows.Controls;

namespace SafeGuardXUltimate.Controls;

public partial class AnimatedRingControl : UserControl
{
    public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(
        nameof(Progress), typeof(int), typeof(AnimatedRingControl), new PropertyMetadata(0));

    public int Progress
    {
        get => (int)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public AnimatedRingControl() => InitializeComponent();
}
