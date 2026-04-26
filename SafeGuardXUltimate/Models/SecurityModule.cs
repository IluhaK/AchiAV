namespace SafeGuardXUltimate.Models;

public sealed class SecurityModule
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public bool IsEnabled { get; set; } = true;
    public int ProtectionLevel { get; set; } = 80;
}
