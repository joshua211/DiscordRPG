using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class RestComplete : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<CharacterId>
{
    public RestComplete(CharacterId entityId)
    {
        EntityId = entityId;
    }

    public CharacterId EntityId { get; private set; }
}