using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.WeightedRandom;

namespace DiscordRPG.Application.Generators;

public class RarityGenerator : GeneratorBase, IRarityGenerator
{
    public Rarity GenerateRarityFromActivityDuration(ActivityDuration duration)
    {
        var selector = new DynamicRandomSelector<Rarity>();
        switch (duration)
        {
            case ActivityDuration.Quick:
                selector.Add(Rarity.Common, 1000);
                selector.Add(Rarity.Uncommon, 100);
                selector.Add(Rarity.Rare, 10);
                selector.Add(Rarity.Unique, 1);
                selector.Add(Rarity.Legendary, 0.1f);
                selector.Add(Rarity.Mythic, 0.01f);
                selector.Add(Rarity.Divine, 0.001f);
                break;
            case ActivityDuration.Short:
                selector.Add(Rarity.Common, 500);
                selector.Add(Rarity.Uncommon, 500);
                selector.Add(Rarity.Rare, 100);
                selector.Add(Rarity.Unique, 1);
                selector.Add(Rarity.Legendary, 0.1f);
                selector.Add(Rarity.Mythic, 0.01f);
                selector.Add(Rarity.Divine, 0.001f);
                break;
            case ActivityDuration.Medium:
                selector.Add(Rarity.Common, 100);
                selector.Add(Rarity.Uncommon, 200);
                selector.Add(Rarity.Rare, 300);
                selector.Add(Rarity.Unique, 100);
                selector.Add(Rarity.Legendary, 5);
                selector.Add(Rarity.Mythic, 0.1f);
                selector.Add(Rarity.Divine, 0.01f);
                break;
            case ActivityDuration.Long:
                selector.Add(Rarity.Common, 10);
                selector.Add(Rarity.Uncommon, 50);
                selector.Add(Rarity.Rare, 100);
                selector.Add(Rarity.Unique, 700);
                selector.Add(Rarity.Legendary, 50);
                selector.Add(Rarity.Mythic, 10f);
                selector.Add(Rarity.Divine, 0.1f);
                break;
            case ActivityDuration.ExtraLong:
                selector.Add(Rarity.Common, 10);
                selector.Add(Rarity.Uncommon, 10);
                selector.Add(Rarity.Rare, 100);
                selector.Add(Rarity.Unique, 600);
                selector.Add(Rarity.Legendary, 200);
                selector.Add(Rarity.Mythic, 100);
                selector.Add(Rarity.Divine, 10);
                break;
        }

        selector.Build();

        return selector.SelectRandomItem();
    }
}