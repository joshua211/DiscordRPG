using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class CharacterCreated : AggregateEvent<GuildAggregate, GuildId>
{
    public CharacterCreated(Character character)
    {
        Character = character;
    }

    public Character Character { get; private set; }
}