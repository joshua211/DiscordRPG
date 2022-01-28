using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Guild.Events;

public class GuildDeleted : AggregateEvent<GuildAggregate, GuildId>
{
}