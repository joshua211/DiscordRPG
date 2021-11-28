using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Events;

public class DungeonExplorationsDecreased : DomainEvent
{
    public DungeonExplorationsDecreased(Dungeon dungeon)
    {
        Dungeon = dungeon;
    }

    public Dungeon Dungeon { get; private set; }
}