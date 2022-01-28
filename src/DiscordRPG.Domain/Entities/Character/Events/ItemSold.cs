using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Character.Events;

public class ItemSold : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<CharacterId>
{
    public ItemSold(CharacterId entityId, ItemId itemId)
    {
        EntityId = entityId;
        ItemId = itemId;
    }

    public ItemId ItemId { get; private set; }

    public CharacterId EntityId { get; private set; }
}