using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Character;

public class CharacterState : AggregateState<CharacterAggregate, CharacterId, CharacterState>
{
}