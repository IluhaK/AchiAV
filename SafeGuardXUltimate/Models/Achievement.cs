namespace SafeGuardXUltimate.Models;

public sealed class Achievement
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public bool IsUnlocked { get; set; }
}
