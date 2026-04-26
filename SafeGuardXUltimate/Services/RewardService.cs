namespace SafeGuardXUltimate.Services;

public sealed class RewardService
{
    public int GetDailyReward(int loginStreak) => 25 + loginStreak * 10;
    public int GetBossReward(int bossLevel) => 100 + bossLevel * 35;
    public int GetPrestigeMultiplier(int prestigeLevel) => 1 + prestigeLevel;
}
