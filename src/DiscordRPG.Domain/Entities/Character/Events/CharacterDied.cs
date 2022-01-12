using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class CharacterDied : AggregateEvent<GuildAggregate, GuildId>
{
    public CharacterDied(CharacterId characterId)
    {
        CharacterId = characterId;
    }

    public CharacterId CharacterId { get; private set; }
}