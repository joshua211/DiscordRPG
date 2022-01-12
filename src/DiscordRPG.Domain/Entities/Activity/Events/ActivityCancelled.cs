using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Activity.Events;

public class ActivityCancelled : AggregateEvent<GuildAggregate, GuildId>
{
    public ActivityCancelled(ActivityId activityId)
    {
        ActivityId = activityId;
    }

    public ActivityId ActivityId { get; private set; }
}