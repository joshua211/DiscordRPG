using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Activity.Events;

public class ActivityCompleted : AggregateEvent<GuildAggregate, GuildId>
{
    public ActivityCompleted(ActivityId activityId, bool wasSuccessfully)
    {
        ActivityId = activityId;
        WasSuccessfully = wasSuccessfully;
    }

    public ActivityId ActivityId { get; private set; }
    public bool WasSuccessfully { get; private set; }
}