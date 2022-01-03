namespace DiscordRPG.Application.Generators;

public class WorthCalculator : IWorthCalculator
{
    public int CalculateWorth(Rarity rarity, uint level) => (int) (level * 10 * (1 + ((int) rarity * 0.2f)));
}

public interface IWorthCalculator
{
    int CalculateWorth(Rarity rarity, uint level);
}