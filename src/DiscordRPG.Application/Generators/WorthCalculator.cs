using DiscordRPG.Domain.Enums;

namespace DiscordRPG.Application.Generators;

public class WorthCalculator : IWorthCalculator
{
    public int CalculateWorth(Rarity rarity, uint level) => (int) (level * 10 * (1 + ((int) rarity * 0.3f)));
}

public interface IWorthCalculator
{
    int CalculateWorth(Rarity rarity, uint level);
}