using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using DiscordRPG.Domain.Entities.Shop.ValueObjects;
using EventFlow.Entities;

namespace DiscordRPG.Domain.Entities.Shop;

public class Shop : Entity<ShopId>
{
    public Shop(ShopId id, List<SalesInventory> inventory) : base(id)
    {
        Inventory = inventory;
    }

    public List<SalesInventory> Inventory { get; private set; }


    public void UpdateInventoryForCharacter(CharacterId characterId, List<Item> newItems)
    {
        var charInventory = Inventory.First(i => i.CharacterId == characterId);
        Inventory[Inventory.IndexOf(charInventory)] = charInventory.UpdateItems(newItems);
    }

    public void RemoveInventory(CharacterId characterId)
    {
        Inventory.RemoveAll(i => i.CharacterId == characterId);
    }
}