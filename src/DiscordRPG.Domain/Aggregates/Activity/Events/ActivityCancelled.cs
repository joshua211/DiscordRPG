using DiscordRPG.Common;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Activity.Events;

public class ActivityCancelled : AggregateEvent<ActivityAggregate, ActivityId>,
    IEntityEvent<Entities.Activity.ActivityId>
{
    public ActivityCancelled(Entities.Activity.ActivityId entityId)
    {
        EntityId = entityId;
    }

    public Entities.Activity.ActivityId EntityId { get; private set; }
}