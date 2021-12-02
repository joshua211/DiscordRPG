using DiscordRPG.Application.Interfaces.Generators;
using DiscordRPG.WeightedRandom;

namespace DiscordRPG.Application.Generators;

public class DungeonGenerator : GeneratorBase, IDungeonGenerator
{
    private readonly INameGenerator nameGenerator;

    public DungeonGenerator(INameGenerator nameGenerator)
    {
        this.nameGenerator = nameGenerator;
    }

    public Dungeon GenerateRandomDungeon(DiscordId serverId, DiscordId threadId, uint charLevel, Rarity rarity)
    {
        var explorations = (byte) random.Next(3, 15);
        var selector = new DynamicRandomSelector<uint>();
        selector.Add(charLevel, 2);
        selector.Add(charLevel - 1, 1.5f);
        selector.Add(charLevel - 2, 1);
        selector.Add(charLevel + 1, 1.5f);
        selector.Add(charLevel + 2, 1);
        var level = selector.Build().SelectRandomItem();
        level = level <= 0 ? 1 : level;

        var name = nameGenerator.GenerateDungeonName(rarity);
        var dungeon = new Dungeon(serverId, threadId, level, rarity, name, explorations);

        return dungeon;
    }
}