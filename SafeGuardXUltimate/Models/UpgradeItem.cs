namespace SafeGuardXUltimate.Models;

public sealed class UpgradeItem
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public int Level { get; set; }
    public int BaseCost { get; init; }
}
