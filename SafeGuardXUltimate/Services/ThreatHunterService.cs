namespace SafeGuardXUltimate.Services;

public sealed class ThreatHunterService
{
    public int GetPassiveIncome(int drones, int aiDefenders) => drones * 2 + aiDefenders * 5;
}
