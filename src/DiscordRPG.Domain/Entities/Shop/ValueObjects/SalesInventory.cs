using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Entities.Shop.ValueObjects;

public class SalesInventory : ValueObject
{
    public SalesInventory(CharacterId characterId, List<Item> itemsForSale)
    {
        CharacterId = characterId;
        ItemsForSale = itemsForSale;
    }

    public CharacterId CharacterId { get; private set; }
    public List<Item> ItemsForSale { get; private set; }

    public SalesInventory UpdateItems(List<Item> newItems)
    {
        return new SalesInventory(CharacterId, newItems);
    }
}