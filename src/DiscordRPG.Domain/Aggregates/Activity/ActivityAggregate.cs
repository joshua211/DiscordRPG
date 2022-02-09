using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Activity;

public class ActivityAggregate : AggregateRoot<ActivityAggregate, ActivityId>
{
    public ActivityAggregate(ActivityId id) : base(id)
    {
    }
}