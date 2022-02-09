using DiscordRPG.Common;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Activity.Events;

public class ActivityCompleted : AggregateEvent<ActivityAggregate, ActivityId>,
    IEntityEvent<Entities.Activity.ActivityId>
{
    public ActivityCompleted(Entities.Activity.ActivityId entityId, bool wasSuccessfully)
    {
        EntityId = entityId;
        WasSuccessfully = wasSuccessfully;
    }

    public bool WasSuccessfully { get; private set; }

    public Entities.Activity.ActivityId EntityId { get; private set; }
}