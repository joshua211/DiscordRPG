namespace DiscordRPG.Application.Generators;

public class WorthCalculator : IWorthCalculator
{
    public int GetItemWorth(Rarity rarity, uint level) => (int) (level * 10 * (1 + ((int) rarity * 0.2f)));
}

public interface IWorthCalculator
{
    int GetItemWorth(Rarity rarity, uint level);
}