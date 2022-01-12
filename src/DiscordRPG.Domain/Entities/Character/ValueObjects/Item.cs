using DiscordRPG.Domain.Entities.Character.Enums;
using DiscordRPG.Domain.Enums;
using EventFlow.Core;
using EventFlow.Entities;
using EventFlow.Exceptions;

namespace DiscordRPG.Domain.Entities.Character.ValueObjects;

public class Item : Entity<ItemId>
{
    public Item(ItemId id, ItemId itemId, string name, string description, int amount, Rarity rarity,
        EquipmentCategory equipmentCategory, EquipmentPosition position, ItemType itemType,
        CharacterAttribute damageAttribute, DamageType damageType, int worth, uint level, bool isUsable, int armor,
        int magicArmor, int strength, int vitality, int agility, int intelligence, int luck, int damageValue,
        bool isEquipped) : base(id)
    {
        if (string.IsNullOrEmpty(name))
            DomainError.With(nameof(name));

        ItemId = itemId;
        Name = name;
        Description = description;
        Amount = amount;
        Rarity = rarity;
        EquipmentCategory = equipmentCategory;
        Position = position;
        ItemType = itemType;
        DamageAttribute = damageAttribute;
        DamageType = damageType;
        Worth = worth;
        Level = level;
        IsUsable = isUsable;
        Armor = armor;
        MagicArmor = magicArmor;
        Strength = strength;
        Vitality = vitality;
        Agility = agility;
        Intelligence = intelligence;
        Luck = luck;
        DamageValue = damageValue;
        IsEquipped = isEquipped;
    }

    public ItemId ItemId { get; private set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Amount { get; set; }
    public Rarity Rarity { get; set; }
    public EquipmentCategory EquipmentCategory { get; private set; }
    public EquipmentPosition Position { get; private set; }
    public ItemType ItemType { get; private set; }
    public CharacterAttribute DamageAttribute { get; private set; }
    public DamageType DamageType { get; private set; }
    public int Worth { get; set; }
    public uint Level { get; set; }
    public bool IsUsable { get; set; }
    public int Armor { get; private set; }
    public int MagicArmor { get; private set; }
    public int Strength { get; private set; }
    public int Vitality { get; private set; }
    public int Agility { get; private set; }
    public int Intelligence { get; private set; }
    public int Luck { get; private set; }
    public int DamageValue { get; private set; }
    public bool IsEquipped { get; private set; }

    public void Equip()
    {
        IsEquipped = true;
    }

    public void Unequip()
    {
        IsEquipped = false;
    }
}

public class ItemId : Identity<ItemId>
{
    public ItemId(string value) : base(value)
    {
    }
}