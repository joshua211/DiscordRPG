using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Activity.Events;

public class ActivityAdded : AggregateEvent<GuildAggregate, GuildId>
{
    public ActivityAdded(Activity activity)
    {
        Activity = activity;
    }

    public Activity Activity { get; private set; }
}