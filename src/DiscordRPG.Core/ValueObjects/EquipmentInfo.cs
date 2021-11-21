using MongoDB.Bson.Serialization.Attributes;

namespace DiscordRPG.Core.ValueObjects;

public class EquipmentInfo
{
    private readonly Dictionary<string, Equipment?> equipment;

    [BsonConstructor]
    public EquipmentInfo(Weapon weapon, Equipment headArmor, Equipment bodyArmor, Equipment legArmor, Equipment amulet,
        Equipment ring1, Equipment ring2)
    {
        equipment = new Dictionary<string, Equipment?>();
        Weapon = weapon;
        HeadArmor = headArmor;
        BodyArmor = bodyArmor;
        LegArmor = legArmor;
        Amulet = amulet;
        Ring1 = ring1;
        Ring2 = ring2;
    }

    public Weapon? Weapon
    {
        get
        {
            equipment.TryGetValue(Equipment.WeaponEquipment, out var eq);
            return eq as Weapon;
        }
        set => equipment[Equipment.WeaponEquipment] = value;
    }

    public Equipment? HeadArmor
    {
        get
        {
            equipment.TryGetValue(Equipment.HeadEquipment, out var eq);
            return eq;
        }
        set => equipment[Equipment.HeadEquipment] = value;
    }

    public Equipment? BodyArmor
    {
        get
        {
            equipment.TryGetValue(Equipment.BodyEquipment, out var eq);
            return eq;
        }
        set => equipment[Equipment.BodyEquipment] = value;
    }

    public Equipment? LegArmor
    {
        get
        {
            equipment.TryGetValue(Equipment.LegEquipment, out var eq);
            return eq;
        }
        set => equipment[Equipment.LegEquipment] = value;
    }

    public Equipment? Amulet
    {
        get
        {
            equipment.TryGetValue(Equipment.Amulet, out var eq);
            return eq;
        }
        set => equipment[Equipment.Amulet] = value;
    }

    public Equipment? Ring1
    {
        get
        {
            equipment.TryGetValue(Equipment.Ring1, out var eq);
            return eq;
        }
        set => equipment[Equipment.Ring1] = value;
    }

    public Equipment? Ring2
    {
        get
        {
            equipment.TryGetValue(Equipment.Ring2, out var eq);
            return eq;
        }
        set => equipment[Equipment.Ring2] = value;
    }

    public int GetTotalAttribute(CharacterAttribute attr) => attr switch
    {
        CharacterAttribute.Strength => equipment.Values.Where(e => e != null).Sum(e => e.Strength),
        CharacterAttribute.Agility => equipment.Values.Where(e => e != null).Sum(e => e.Agility),
        CharacterAttribute.Intelligence => equipment.Values.Where(e => e != null).Sum(e => e.Intelligence),
        CharacterAttribute.Vitality => equipment.Values.Where(e => e != null).Sum(e => e.Vitality),
        CharacterAttribute.Luck => equipment.Values.Where(e => e != null).Sum(e => e.Luck),
        _ => 0
    };

    public int GetTotalArmor() => equipment.Values.Where(e => e != null).Sum(e => e.Armor);

    public int GetTotalMagicArmor() => equipment.Values.Where(e => e != null).Sum(e => e.MagicArmor);
}