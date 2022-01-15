using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Guild;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Entities.Shop.Events;

public class ShopAdded : AggregateEvent<GuildAggregate, GuildId>, IEntityEvent<ShopId>
{
    public ShopAdded(Shop shop)
    {
        Shop = shop;
    }

    public Shop Shop { get; private set; }
    public ShopId EntityId => Shop.Id;
}