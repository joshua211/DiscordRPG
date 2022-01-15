using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Dungeon.Events;

public class ExplorationsDecreased : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<DungeonId>
{
    public ExplorationsDecreased(DungeonId entityId)
    {
        EntityId = entityId;
    }

    public DungeonId EntityId { get; private set; }
}