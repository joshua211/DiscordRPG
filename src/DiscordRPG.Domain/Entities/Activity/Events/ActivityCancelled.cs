using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Activity.Events;

public class ActivityCancelled : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<ActivityId>
{
    public ActivityCancelled(ActivityId entityId)
    {
        EntityId = entityId;
    }

    public ActivityId EntityId { get; private set; }
}