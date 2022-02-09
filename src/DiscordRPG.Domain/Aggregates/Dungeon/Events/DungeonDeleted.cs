using DiscordRPG.Common;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Dungeon.Events;

public class DungeonDeleted : AggregateEvent<DungeonAggregate, DungeonId>, IEntityEvent<DungeonId>
{
    public DungeonDeleted(DungeonId entityId)
    {
        EntityId = entityId;
    }

    public DungeonId EntityId { get; private set; }
}