using DiscordRPG.Domain.Entities.Character.Enums;
using DiscordRPG.Domain.Enums;
using EventFlow.Core;
using EventFlow.Entities;
using EventFlow.Exceptions;

namespace DiscordRPG.Domain.Entities.Character.ValueObjects;

public class Item : Entity<ItemId>
{
    public Item(ItemId id, string name, string description, int amount, Rarity rarity,
        EquipmentCategory equipmentCategory, EquipmentPosition position, ItemType itemType,
        CharacterAttribute damageAttribute, DamageType damageType, StatusEffect itemEffect, int worth, uint level,
        int armor,
        int magicArmor, int strength, int vitality, int agility, int intelligence, int luck, int damageValue,
        bool isEquipped) : base(id)
    {
        if (string.IsNullOrEmpty(name))
            DomainError.With(nameof(name));

        Name = name;
        Description = description;
        Amount = amount;
        Rarity = rarity;
        EquipmentCategory = equipmentCategory;
        Position = position;
        ItemType = itemType;
        DamageAttribute = damageAttribute;
        DamageType = damageType;
        ItemEffect = itemEffect;
        Worth = worth;
        Level = level;
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

    public string Name { get; private set; }
    public string Description { get; private set; }
    public int Amount { get; private set; }
    public Rarity Rarity { get; private set; }
    public EquipmentCategory EquipmentCategory { get; private set; }
    public EquipmentPosition Position { get; private set; }
    public ItemType ItemType { get; private set; }
    public CharacterAttribute DamageAttribute { get; private set; }
    public DamageType DamageType { get; private set; }
    public StatusEffect ItemEffect { get; private set; }
    public int Worth { get; private set; }
    public uint Level { get; private set; }
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

    public void IncreaseAmount(int genItemAmount)
    {
        Amount += genItemAmount;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        return new object[]
        {
            Name, Worth, Level, Rarity, Armor, MagicArmor, Strength, Vitality, Agility, Intelligence, Luck, DamageValue,
            DamageType, DamageAttribute
        };
    }

    public override string ToString() => ItemType switch
    {
        _ => $"[{Rarity} {ItemType}] {Name} (Lvl: {Level} | {Worth}$)"
    };
}

public class ItemId : Identity<ItemId>
{
    public ItemId(string value) : base(value)
    {
    }

    public override string ToString()
    {
        return Value;
    }
}