using DiscordRPG.Common;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Activity.Events;

public class ActivityAdded : AggregateEvent<ActivityAggregate, ActivityId>, IEntityEvent<Entities.Activity.ActivityId>
{
    public ActivityAdded(Entities.Activity.Activity activity)
    {
        Activity = activity;
    }

    public Entities.Activity.Activity Activity { get; private set; }
    public Entities.Activity.ActivityId EntityId => Activity.Id;
}