using SafeGuardXUltimate.Models;
using System.Collections.Generic;

namespace SafeGuardXUltimate.Services;

public sealed class MockDataService
{
    public IReadOnlyList<Achievement> GetAchievements() =>
        new List<Achievement>
        {
            new() { Title = "First Scan", Description = "Run your first quick scan.", IsUnlocked = true },
            new() { Title = "Threat Hunter", Description = "Neutralize 50 simulated threats.", IsUnlocked = false },
            new() { Title = "Legendary Guardian", Description = "Reach rank Sentinel X.", IsUnlocked = false }
        };

    public IReadOnlyList<ShopItem> GetShopItems() =>
        new List<ShopItem>
        {
            new() { Name = "Neon Theme", Rarity = "Rare", Cost = 200 },
            new() { Name = "Crimson Threat Mode", Rarity = "Epic", Cost = 800 },
            new() { Name = "Holo Avatar Pack", Rarity = "Legendary", Cost = 1500 }
        };

    public IReadOnlyList<NewsItem> GetNews() =>
        new List<NewsItem>
        {
            new() { Headline = "Global phishing simulator activity spikes", Category = "Threat Intel" },
            new() { Headline = "Patch Tuesday demo feed updated", Category = "Updates" }
        };
}
