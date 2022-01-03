using DiscordRPG.Core.DomainServices.Generators;
using Weighted_Randomizer;

namespace DiscordRPG.Application.Generators;

public class DungeonGenerator : GeneratorBase, IDungeonGenerator
{
    private readonly IAspectGenerator aspectGenerator;
    private readonly INameGenerator nameGenerator;
    private readonly IRarityGenerator rarityGenerator;

    public DungeonGenerator(INameGenerator nameGenerator, IRarityGenerator rarityGenerator,
        IAspectGenerator aspectGenerator)
    {
        this.nameGenerator = nameGenerator;
        this.rarityGenerator = rarityGenerator;
        this.aspectGenerator = aspectGenerator;
    }

    public Dungeon GenerateRandomDungeon(DiscordId serverId, DiscordId threadId, uint charLevel,
        ActivityDuration duration)
    {
        var rarity = rarityGenerator.GenerateRarityFromActivityDuration(duration);
        var aspect = aspectGenerator.GetRandomAspect(rarity);

        return GenerateRandomDungeon(serverId, threadId, charLevel, aspect, rarity);
    }

    public Dungeon GenerateRandomDungeon(DiscordId serverId, DiscordId threadId, uint charLevel, Aspect aspect,
        Rarity rarity)
    {
        var explorations = (byte) random.Next(3, 15);
        var randomizer = new DynamicWeightedRandomizer<uint>();
        randomizer.Add(charLevel, 2);
        if (charLevel >= 3)
        {
            randomizer.Add(charLevel - 1, 3);
            randomizer.Add(charLevel - 2, 1);
        }

        var level = randomizer.NextWithReplacement();
        level = level <= 0 ? 1 : level;

        var name = aspect.DungeonPrefix + " " + nameGenerator.GenerateDungeonName(rarity);
        var dungeon = new Dungeon(serverId, threadId, level, rarity, name, explorations, aspect);

        return dungeon;
    }
}