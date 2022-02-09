using DiscordRPG.Domain.Aggregates.Activity.Enums;
using DiscordRPG.Domain.Aggregates.Dungeon;
using DiscordRPG.Domain.Aggregates.Dungeon.ValueObjects;
using DiscordRPG.Domain.Enums;
using Weighted_Randomizer;

namespace DiscordRPG.Application.Generators;

public class DungeonGenerator : GeneratorBase
{
    private readonly AspectGenerator aspectGenerator;
    private readonly NameGenerator nameGenerator;
    private readonly RarityGenerator rarityGenerator;

    public DungeonGenerator(NameGenerator nameGenerator, RarityGenerator rarityGenerator,
        AspectGenerator aspectGenerator)
    {
        this.nameGenerator = nameGenerator;
        this.rarityGenerator = rarityGenerator;
        this.aspectGenerator = aspectGenerator;
    }

    public Dungeon GenerateRandomDungeon(DungeonId dungeonId, uint charLevel, int charLuck,
        ActivityDuration duration)
    {
        var rarity = rarityGenerator.GenerateRarityFromActivityDuration(duration, charLuck);
        var aspect = aspectGenerator.GetRandomAspect(rarity);

        return GenerateRandomDungeon(dungeonId, charLevel, aspect, rarity);
    }

    public Dungeon GenerateRandomDungeon(DungeonId dungeonId, uint charLevel, Aspect aspect,
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
        var dungeon = new Dungeon(dungeonId, new DungeonName(name), new Explorations(explorations),
            new DungeonLevel(level), rarity, aspect);

        return dungeon;
    }
}