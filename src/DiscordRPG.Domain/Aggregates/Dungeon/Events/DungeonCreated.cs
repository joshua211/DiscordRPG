using DiscordRPG.Domain.Aggregates.Character;
using DiscordRPG.Domain.Aggregates.Dungeon.ValueObjects;
using DiscordRPG.Domain.Enums;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Dungeon.Events;

public class DungeonCreated : AggregateEvent<DungeonAggregate, DungeonId>
{
    public DungeonCreated(DungeonName name, Explorations explorations, DungeonLevel level, Rarity rarity, Aspect aspect,
        CharacterId owner)
    {
        Name = name;
        Explorations = explorations;
        Level = level;
        Rarity = rarity;
        Aspect = aspect;
        Owner = owner;
    }

    public DungeonName Name { get; private set; }
    public Explorations Explorations { get; private set; }
    public DungeonLevel Level { get; private set; }
    public Rarity Rarity { get; private set; }
    public Aspect Aspect { get; private set; }
    public CharacterId Owner { get; private set; }
}