using SafeGuardXUltimate.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SafeGuardXUltimate.Services;

public sealed class ThemeService
{
    public IReadOnlyList<AppThemeOption> GetThemes() => new List<AppThemeOption>
    {
        new() { Name = "Kaspersky-style Green", ResourcePath = "/SafeGuardXUltimate;component/Styles/Themes/KasperskyGreen.xaml" },
        new() { Name = "Dark Premium", ResourcePath = "/SafeGuardXUltimate;component/Styles/Themes/DarkPremium.xaml" },
        new() { Name = "Blue Enterprise", ResourcePath = "/SafeGuardXUltimate;component/Styles/Themes/BlueEnterprise.xaml" },
        new() { Name = "Cyber Neon", ResourcePath = "/SafeGuardXUltimate;component/Styles/Themes/CyberNeon.xaml" },
        new() { Name = "Red Alert", ResourcePath = "/SafeGuardXUltimate;component/Styles/Themes/RedAlert.xaml" }
    };

    public void ApplyTheme(string resourcePath)
    {
        var app = Application.Current;
        if (app is null) return;

        var existing = app.Resources.MergedDictionaries
            .FirstOrDefault(d => d.Source is not null && d.Source.OriginalString.Contains("Styles/Themes/"));

        if (existing is not null)
        {
            app.Resources.MergedDictionaries.Remove(existing);
        }

        app.Resources.MergedDictionaries.Insert(0, new ResourceDictionary { Source = new Uri(resourcePath, UriKind.RelativeOrAbsolute) });
    }
}
