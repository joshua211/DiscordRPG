using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class CharacterDied : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<CharacterId>
{
    public CharacterDied(CharacterId characterId)
    {
        EntityId = characterId;
    }


    public CharacterId EntityId { get; }
}