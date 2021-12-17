using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Events;

public class DungeonCreated : DomainEvent
{
    public DungeonCreated(Dungeon dungeon, ActivityDuration requestActivityDuration)
    {
        Dungeon = dungeon;
    }

    public Dungeon Dungeon { get; private set; }
}