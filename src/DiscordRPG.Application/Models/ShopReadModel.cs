using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Entities.Shop.Events;
using DiscordRPG.Domain.Entities.Shop.ValueObjects;
using EventFlow.Aggregates;
using EventFlow.MongoDB.ReadStores;
using EventFlow.ReadStores;

namespace DiscordRPG.Application.Models;

public class ShopReadModel : IMongoDbReadModel,
    IAmReadModelFor<GuildAggregate, GuildId, ShopAdded>,
    IAmReadModelFor<GuildAggregate, GuildId, ShopInventoryRemoved>,
    IAmReadModelFor<GuildAggregate, GuildId, ShopInventoryUpdated>,
    IAmReadModelFor<GuildAggregate, GuildId, ItemRemoved>
{
    public List<SalesInventory> Inventory { get; private set; }
    public string GuildId { get; private set; }

    public void Apply(IReadModelContext context, IDomainEvent<GuildAggregate, GuildId, ItemRemoved> domainEvent)
    {
        var charSellInventory = Inventory.First(i => i.CharacterId == domainEvent.AggregateEvent.CharacterId);
        var item = charSellInventory.ItemsForSale.First(i => i.Id == domainEvent.AggregateEvent.ItemId);
        charSellInventory.ItemsForSale.Remove(item);
    }

    public void Apply(IReadModelContext context, IDomainEvent<GuildAggregate, GuildId, ShopAdded> domainEvent)
    {
        var shop = domainEvent.AggregateEvent.Shop;
        Id = shop.Id.Value;
        GuildId = domainEvent.AggregateIdentity.Value;
        Inventory = shop.Inventory;
    }

    public void Apply(IReadModelContext context,
        IDomainEvent<GuildAggregate, GuildId, ShopInventoryRemoved> domainEvent)
    {
        Inventory.RemoveAll(i => i.CharacterId == domainEvent.AggregateEvent.CharacterId);
    }

    public void Apply(IReadModelContext context,
        IDomainEvent<GuildAggregate, GuildId, ShopInventoryUpdated> domainEvent)
    {
        var charInventory = Inventory.FirstOrDefault(i => i.CharacterId == domainEvent.AggregateEvent.CharacterId);
        if (charInventory is null)
        {
            charInventory = new SalesInventory(domainEvent.AggregateEvent.CharacterId,
                domainEvent.AggregateEvent.NewItems);
            Inventory.Add(charInventory);
        }
        else
            Inventory[Inventory.IndexOf(charInventory)] =
                charInventory.UpdateItems(domainEvent.AggregateEvent.NewItems);
    }

    public string Id { get; private set; }
    public long? Version { get; set; }
}