using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class TitleEquipped : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<CharacterId>
{
    public TitleEquipped(TitleId titleId, CharacterId entityId)
    {
        TitleId = titleId;
        EntityId = entityId;
    }

    public TitleId TitleId { get; private set; }
    public CharacterId EntityId { get; }
}