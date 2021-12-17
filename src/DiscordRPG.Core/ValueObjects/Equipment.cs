using MongoDB.Bson.Serialization.Attributes;

namespace DiscordRPG.Core.ValueObjects;

public class Equipment : Item
{
    [BsonConstructor]
    public Equipment(string name, string description, Rarity rarity, int armor, int magicArmor, int strength,
        int vitality, int agility, int intelligence, int luck, int worth, EquipmentCategory equipmentCategory,
        EquipmentPosition position,
        uint level) : base(
        name, description, rarity, worth, level)
    {
        Armor = armor;
        MagicArmor = magicArmor;
        Strength = strength;
        Vitality = vitality;
        Agility = agility;
        Intelligence = intelligence;
        Luck = luck;
        EquipmentCategory = equipmentCategory;
        Position = position;
    }

    public int Armor { get; private set; }
    public int MagicArmor { get; private set; }
    public EquipmentCategory EquipmentCategory { get; private set; }
    public EquipmentPosition Position { get; private set; }
    public int Strength { get; private set; }
    public int Vitality { get; private set; }
    public int Agility { get; private set; }
    public int Intelligence { get; private set; }
    public int Luck { get; private set; }

    public override string ToString()
    {
        return $"[{Rarity.ToString()} Equipment] {Name} (Lvl: {Level} | {Worth}$)";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Equipment equipment)
            return false;

        return equipment.GetHashCode() == GetHashCode();
    }

    public override int GetHashCode()
    {
        return Strength.GetHashCode() + Vitality.GetHashCode() + Agility.GetHashCode() + Intelligence.GetHashCode() +
               Luck.GetHashCode() + Name.GetHashCode() + Level.GetHashCode() + Rarity.GetHashCode();
    }
}