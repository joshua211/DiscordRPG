using MongoDB.Bson.Serialization.Attributes;

namespace DiscordRPG.Core.ValueObjects;

public class EquipmentInfo
{
    public readonly Dictionary<EquipmentPosition, Equipment?> CurrentEquipment;

    [BsonConstructor]
    public EquipmentInfo(Weapon weapon, Equipment helmet, Equipment armor, Equipment pants, Equipment amulet,
        Equipment ring)
    {
        CurrentEquipment = new Dictionary<EquipmentPosition, Equipment?>();
        Weapon = weapon;
        Helmet = helmet;
        Armor = armor;
        Pants = pants;
        Amulet = amulet;
        Ring = ring;
    }

    public Weapon? Weapon
    {
        get
        {
            CurrentEquipment.TryGetValue(EquipmentPosition.Weapon, out var eq);
            return eq as Weapon;
        }
        set => CurrentEquipment[EquipmentPosition.Weapon] = value;
    }

    public Equipment? Helmet
    {
        get
        {
            CurrentEquipment.TryGetValue(EquipmentPosition.Helmet, out var eq);
            return eq;
        }
        set => CurrentEquipment[EquipmentPosition.Helmet] = value;
    }

    public Equipment? Armor
    {
        get
        {
            CurrentEquipment.TryGetValue(EquipmentPosition.Armor, out var eq);
            return eq;
        }
        set => CurrentEquipment[EquipmentPosition.Armor] = value;
    }

    public Equipment? Pants
    {
        get
        {
            CurrentEquipment.TryGetValue(EquipmentPosition.Pants, out var eq);
            return eq;
        }
        set => CurrentEquipment[EquipmentPosition.Pants] = value;
    }

    public Equipment? Amulet
    {
        get
        {
            CurrentEquipment.TryGetValue(EquipmentPosition.Amulet, out var eq);
            return eq;
        }
        set => CurrentEquipment[EquipmentPosition.Amulet] = value;
    }

    public Equipment? Ring
    {
        get
        {
            CurrentEquipment.TryGetValue(EquipmentPosition.Ring, out var eq);
            return eq;
        }
        set => CurrentEquipment[EquipmentPosition.Ring] = value;
    }

    public int GetTotalAttribute(CharacterAttribute attr) => attr switch
    {
        CharacterAttribute.Strength => CurrentEquipment.Values.Where(e => e != null).Sum(e => e.Strength),
        CharacterAttribute.Agility => CurrentEquipment.Values.Where(e => e != null).Sum(e => e.Agility),
        CharacterAttribute.Intelligence => CurrentEquipment.Values.Where(e => e != null).Sum(e => e.Intelligence),
        CharacterAttribute.Vitality => CurrentEquipment.Values.Where(e => e != null).Sum(e => e.Vitality),
        CharacterAttribute.Luck => CurrentEquipment.Values.Where(e => e != null).Sum(e => e.Luck),
        _ => 0
    };

    public int GetTotalArmor() => CurrentEquipment.Values.Where(e => e != null).Sum(e => e.Armor);

    public int GetTotalMagicArmor() => CurrentEquipment.Values.Where(e => e != null).Sum(e => e.MagicArmor);
}