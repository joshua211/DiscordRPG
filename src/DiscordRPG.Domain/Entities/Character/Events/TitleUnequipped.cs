using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class TitleUnequipped : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<CharacterId>
{
    public TitleUnequipped(CharacterId entityId, TitleId titleId)
    {
        EntityId = entityId;
        TitleId = titleId;
    }

    public TitleId TitleId { get; private set; }
    public CharacterId EntityId { get; }
}