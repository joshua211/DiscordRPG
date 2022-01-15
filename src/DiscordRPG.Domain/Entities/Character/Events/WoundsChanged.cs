using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class WoundsChanged : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<CharacterId>
{
    public WoundsChanged(CharacterId entityId, List<Wound> newWounds)
    {
        EntityId = entityId;
        NewWounds = newWounds;
    }

    public List<Wound> NewWounds { get; private set; }

    public CharacterId EntityId { get; private set; }
}