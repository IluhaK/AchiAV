namespace SafeGuardXUltimate.Models;

public sealed class ThreatItem
{
    public required string Name { get; init; }
    public required string Severity { get; init; }
    public required string Path { get; init; }
    public bool IsNeutralized { get; set; }
}
