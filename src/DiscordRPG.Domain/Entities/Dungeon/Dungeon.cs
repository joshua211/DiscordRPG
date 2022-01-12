using DiscordRPG.Domain.Entities.Dungeon.ValueObjects;
using DiscordRPG.Domain.Enums;
using EventFlow.Entities;

namespace DiscordRPG.Domain.Entities.Dungeon;

public class Dungeon : Entity<DungeonId>
{
    public Dungeon(DungeonId id, DungeonName name, Explorations explorations, DungeonLevel level,
        Rarity rarity) : base(id)
    {
        Name = name;
        Explorations = explorations;
        Level = level;
        Rarity = rarity;
    }

    public DungeonName Name { get; private set; }
    public Explorations Explorations { get; private set; }
    public DungeonLevel Level { get; private set; }
    public Rarity Rarity { get; private set; }

    public void DecreaseExplorations()
    {
        Explorations = Explorations.Decrease();
    }
}