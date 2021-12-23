namespace DiscordRPG.Core.Entities;

public class Shop : Entity
{
    public Shop(Identity guildId, List<ShopInventory> itemsForSale)
    {
        GuildId = guildId;
        ItemsForSale = itemsForSale;
    }

    public Identity GuildId { get; private set; }
    public List<ShopInventory> ItemsForSale { get; private set; }

    public List<Equipment> this[Identity charId]
    {
        get => ItemsForSale.FirstOrDefault(l => Equals(l.CharId, charId)).Equipment;
        set
        {
            var inv = ItemsForSale.FirstOrDefault(l => Equals(l.CharId, charId));
            if (inv is null)
                ItemsForSale.Add(new ShopInventory(charId, value));
            else
                ItemsForSale[ItemsForSale.IndexOf(inv)] = new ShopInventory(charId, value);
        }
    }

    public void UpdateEquipment(Identity charID, List<Equipment> equipment)
    {
        this[charID] = equipment;
    }
}