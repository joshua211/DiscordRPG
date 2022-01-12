using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Shop.Events;

public class ShopInventoryUpdated : AggregateEvent<GuildAggregate, GuildId>
{
    public ShopInventoryUpdated(ShopId shopId, CharacterId characterId, List<Item> newItems)
    {
        ShopId = shopId;
        CharacterId = characterId;
        NewItems = newItems;
    }

    public ShopId ShopId { get; private set; }
    public CharacterId CharacterId { get; private set; }
    public List<Item> NewItems { get; private set; }
}