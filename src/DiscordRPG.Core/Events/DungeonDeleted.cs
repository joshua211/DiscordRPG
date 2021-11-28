namespace DiscordRPG.Core.Events;

public class DungeonDeleted : DomainEvent
{
    public DungeonDeleted(Identity dungeonId)
    {
        DungeonId = dungeonId;
    }

    public Identity DungeonId { get; private set; }
}