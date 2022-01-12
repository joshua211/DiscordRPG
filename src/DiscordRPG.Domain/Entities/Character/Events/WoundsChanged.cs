using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class WoundsChanged : AggregateEvent<GuildAggregate, GuildId>
{
    public WoundsChanged(CharacterId characterId, List<Wound> newWounds)
    {
        CharacterId = characterId;
        NewWounds = newWounds;
    }

    public CharacterId CharacterId { get; private set; }
    public List<Wound> NewWounds { get; private set; }
}