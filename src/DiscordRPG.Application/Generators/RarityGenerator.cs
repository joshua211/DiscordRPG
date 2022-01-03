using DiscordRPG.Core.DomainServices.Generators;
using Weighted_Randomizer;

namespace DiscordRPG.Application.Generators;

public class RarityGenerator : GeneratorBase, IRarityGenerator
{
    public Rarity GenerateRarityFromActivityDuration(ActivityDuration duration)
    {
        var selector = new DynamicWeightedRandomizer<int>();
        switch (duration)
        {
            case ActivityDuration.Quick:
                selector.Add((int) Rarity.Common, 1000000);
                selector.Add((int) Rarity.Uncommon, 100000);
                selector.Add((int) Rarity.Rare, 10000);
                selector.Add((int) Rarity.Unique, 1000);
                selector.Add((int) Rarity.Legendary, 100);
                selector.Add((int) Rarity.Mythic, 10);
                selector.Add((int) Rarity.Divine, 1);
                break;
            case ActivityDuration.Short:
                selector.Add((int) Rarity.Common, 500000);
                selector.Add((int) Rarity.Uncommon, 500000);
                selector.Add((int) Rarity.Rare, 100000);
                selector.Add((int) Rarity.Unique, 1000);
                selector.Add((int) Rarity.Legendary, 100);
                selector.Add((int) Rarity.Mythic, 100);
                selector.Add((int) Rarity.Divine, 1);
                break;
            case ActivityDuration.Medium:
                selector.Add((int) Rarity.Common, 10000);
                selector.Add((int) Rarity.Uncommon, 20000);
                selector.Add((int) Rarity.Rare, 30000);
                selector.Add((int) Rarity.Unique, 10000);
                selector.Add((int) Rarity.Legendary, 500);
                selector.Add((int) Rarity.Mythic, 10);
                selector.Add((int) Rarity.Divine, 1);
                break;
            case ActivityDuration.Long:
                selector.Add((int) Rarity.Common, 100);
                selector.Add((int) Rarity.Uncommon, 500);
                selector.Add((int) Rarity.Rare, 1000);
                selector.Add((int) Rarity.Unique, 7000);
                selector.Add((int) Rarity.Legendary, 500);
                selector.Add((int) Rarity.Mythic, 100);
                selector.Add((int) Rarity.Divine, 1);
                break;
            case ActivityDuration.ExtraLong:
                selector.Add((int) Rarity.Common, 10);
                selector.Add((int) Rarity.Uncommon, 10);
                selector.Add((int) Rarity.Rare, 100);
                selector.Add((int) Rarity.Unique, 600);
                selector.Add((int) Rarity.Legendary, 200);
                selector.Add((int) Rarity.Mythic, 100);
                selector.Add((int) Rarity.Divine, 10);
                break;
        }

        return (Rarity) selector.NextWithReplacement();
    }

    public Rarity GenerateShopRarity()
    {
        var selector = new DynamicWeightedRandomizer<int>();
        selector.Add((int) Rarity.Common, 10);
        selector.Add((int) Rarity.Uncommon, 7);
        selector.Add((int) Rarity.Rare, 3);
        selector.Add((int) Rarity.Unique, 1);

        return (Rarity) selector.NextWithReplacement();
    }
}