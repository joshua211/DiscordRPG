using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.WeightedRandom;

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

        var explorations = (byte) random.Next(3, 15);
        var selector = new DynamicRandomSelector<uint>();
        selector.Add(charLevel, 2);
        selector.Add(charLevel - 1, 1.5f);
        selector.Add(charLevel - 2, 1);
        selector.Add(charLevel + 1, 1.5f);
        selector.Add(charLevel + 2, 1);
        var level = selector.Build().SelectRandomItem();
        level = level <= 0 ? 1 : level;

        var name = aspect.DungeonPrefix + " " + nameGenerator.GenerateDungeonName(rarity);
        var dungeon = new Dungeon(serverId, threadId, level, rarity, name, explorations, aspect);

        return dungeon;
    }
}