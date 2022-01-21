using DiscordRPG.Domain.Enums;

namespace DiscordRPG.Domain.DomainServices;

public class HealthPotionCalculator : IHealthPotionCalculator
{
    public int CalculateHealAmount(Rarity rarity, uint level)
    {
        return (int) Math.Round(level * 20 * (1 + (int) rarity * 0.2f));
    }
}

public interface IHealthPotionCalculator
{
    int CalculateHealAmount(Rarity rarity, uint level);
}