using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Events;

public class ItemBought : DomainEvent
{
    public ItemBought(Character character, Item item)
    {
        Character = character;
        Item = item;
    }

    public Character Character { get; private set; }
    public Item Item { get; set; }
}