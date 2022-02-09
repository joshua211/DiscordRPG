using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Character.Events;

public class CharacterDied : AggregateEvent<CharacterAggregate, CharacterId>
{
    public CharacterDied()
    {
    }
}