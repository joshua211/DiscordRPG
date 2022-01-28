using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Shop.Events;

public class ShopInventoryUpdated : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<ShopId>
{
    public ShopInventoryUpdated(ShopId entityId, CharacterId characterId, List<Item> newItems)
    {
        EntityId = entityId;
        CharacterId = characterId;
        NewItems = newItems;
    }

    public CharacterId CharacterId { get; private set; }
    public List<Item> NewItems { get; private set; }

    public ShopId EntityId { get; private set; }
}