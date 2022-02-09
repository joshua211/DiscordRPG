using DiscordRPG.Application.Data;
using DiscordRPG.Domain.Aggregates.Dungeon.ValueObjects;
using DiscordRPG.Domain.Enums;

namespace DiscordRPG.Application.Generators;

public class AspectGenerator : GeneratorBase
{
    public Aspect GetRandomAspect(Rarity rarity)
    {
        var byRarity = Aspects.AspectsByRarity[rarity];

        return byRarity[random.Next(byRarity.Count)];
    }
}