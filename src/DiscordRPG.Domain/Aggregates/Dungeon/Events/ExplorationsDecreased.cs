using DiscordRPG.Common;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Dungeon.Events;

public class ExplorationsDecreased : AggregateEvent<DungeonAggregate, DungeonId>, IEntityEvent<DungeonId>
{
    public ExplorationsDecreased(DungeonId entityId)
    {
        EntityId = entityId;
    }

    public DungeonId EntityId { get; private set; }
}