using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Dungeon.Events;

public class DungeonDeleted : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<DungeonId>
{
    public DungeonDeleted(DungeonId entityId)
    {
        EntityId = entityId;
    }

    public DungeonId EntityId { get; private set; }
}