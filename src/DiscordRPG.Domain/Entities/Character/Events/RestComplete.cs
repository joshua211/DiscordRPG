using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class RestComplete : AggregateEvent<GuildAggregate, GuildId>
{
    public RestComplete(CharacterId characterId)
    {
        CharacterId = characterId;
    }

    public CharacterId CharacterId { get; private set; }
}