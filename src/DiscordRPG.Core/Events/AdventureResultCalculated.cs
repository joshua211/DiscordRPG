using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Events;

public class AdventureResultCalculated : DomainEvent
{
    public AdventureResultCalculated(DungeonResult dungeonResult, Character character, Dungeon dungeon)
    {
        DungeonResult = dungeonResult;
        Character = character;
        Dungeon = dungeon;
    }

    public DungeonResult DungeonResult { get; private set; }
    public Character Character { get; private set; }
    public Dungeon Dungeon { get; private set; }
}