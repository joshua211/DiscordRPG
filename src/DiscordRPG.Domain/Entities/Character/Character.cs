using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.Entities;

namespace DiscordRPG.Domain.Entities.Character;

public class Character : Entity<CharacterId>
{
    public Character(CharacterId id, CharacterClass @class, CharacterRace race, CharacterName name,
        Level characterLevel, List<Item> inventory, List<Wound> wounds, Money money) : base(id)
    {
        Class = @class;
        Race = race;
        Name = name;
        CharacterLevel = characterLevel;
        Inventory = inventory;
        Wounds = wounds;
        Money = money;
    }

    public CharacterClass Class { get; private set; }
    public CharacterRace Race { get; private set; }
    public CharacterName Name { get; private set; }
    public Level CharacterLevel { get; private set; }
    public Money Money { get; private set; }
    public List<Item> Inventory { get; private set; }
    public List<Wound> Wounds { get; private set; }

    public void ChangeInventory(List<Item> aggregateEventNewInventory)
    {
        Inventory = aggregateEventNewInventory;
    }

    public void AddMoney(int amount)
    {
        Money = Money.Add(amount);
    }

    public void EquipItem(ItemId id)
    {
        var item = Inventory.First(i => i.Id == id);
        var alreadyEquipped = Inventory.FirstOrDefault(i => i.IsEquipped && i.Position == item.Position);
        if (alreadyEquipped is not null)
            alreadyEquipped.Unequip();
        item.Equip();
    }

    public void UnequipItem(ItemId id)
    {
        var item = Inventory.First(i => i.Id == id);
        item.Unequip();
    }

    public void SetLevel(Level aggregateEventNewLevel)
    {
        CharacterLevel = aggregateEventNewLevel;
    }

    public void ChangeWounds(List<Wound> aggregateEventNewWounds)
    {
        Wounds = aggregateEventNewWounds;
    }
}