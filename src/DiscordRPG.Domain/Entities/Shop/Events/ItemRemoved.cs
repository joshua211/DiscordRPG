using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Shop.Events;

public class ItemRemoved : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<ShopId>
{
    public ItemRemoved(CharacterId characterId, ItemId itemId, ShopId entityId)
    {
        CharacterId = characterId;
        ItemId = itemId;
        EntityId = entityId;
    }

    public CharacterId CharacterId { get; private set; }
    public ItemId ItemId { get; private set; }
    public ShopId EntityId { get; }
}