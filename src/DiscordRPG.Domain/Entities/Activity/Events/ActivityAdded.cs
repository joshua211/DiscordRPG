using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Activity.Events;

public class ActivityAdded : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<ActivityId>
{
    public ActivityAdded(Activity activity)
    {
        Activity = activity;
    }

    public Activity Activity { get; private set; }
    public ActivityId EntityId => Activity.Id;
}