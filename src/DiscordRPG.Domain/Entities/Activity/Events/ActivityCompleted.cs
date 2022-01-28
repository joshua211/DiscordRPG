using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Activity.Events;

public class ActivityCompleted : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<ActivityId>
{
    public ActivityCompleted(ActivityId entityId, bool wasSuccessfully)
    {
        EntityId = entityId;
        WasSuccessfully = wasSuccessfully;
    }

    public bool WasSuccessfully { get; private set; }

    public ActivityId EntityId { get; private set; }
}