namespace SafeGuardXUltimate.Models;

public sealed class ShopItem
{
    public required string Name { get; init; }
    public required string Rarity { get; init; }
    public int Cost { get; init; }
}
