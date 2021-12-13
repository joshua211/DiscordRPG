using DiscordRPG.Application.Data;
using DiscordRPG.Core.DomainServices.Generators;

namespace DiscordRPG.Application.Generators;

public class AspectGenerator : GeneratorBase, IAspectGenerator
{
    public Aspect GetRandomAspect(Rarity rarity)
    {
        var byRarity = Aspects.AspectsByRarity[rarity];

        return byRarity[random.Next(byRarity.Count)];
    }
}