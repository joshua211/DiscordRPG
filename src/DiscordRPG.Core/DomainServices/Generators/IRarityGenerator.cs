namespace DiscordRPG.Core.DomainServices.Generators;

public interface IRarityGenerator
{
    Rarity GenerateRarityFromActivityDuration(ActivityDuration duration, int luck);
    Rarity GenerateShopRarity();
}