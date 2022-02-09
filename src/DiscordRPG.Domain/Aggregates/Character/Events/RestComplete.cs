using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Character.Events;

public class RestComplete : AggregateEvent<CharacterAggregate, CharacterId>
{
}