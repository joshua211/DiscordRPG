using MongoDB.Bson.Serialization.Attributes;

namespace DiscordRPG.Core.ValueObjects;

public class Equipment : Item
{
    public const string WeaponEquipment = "Weapon";
    public const string HeadEquipment = "HeadArmor";
    public const string BodyEquipment = "BodyArmor";
    public const string LegEquipment = "LegArmor";
    public const string Amulet = "Amulet";
    public const string Ring1 = "Ring1";
    public const string Ring2 = "Ring2";

    [BsonConstructor]
    public Equipment(string name, string description, Rarity rarity, int armor, int magicArmor, int strength,
        int vitality, int agility, int intelligence, int luck, int worth, EquipmentCategory equipmentCategory) : base(
        name, description, rarity, worth)
    {
        Armor = armor;
        MagicArmor = magicArmor;
        Strength = strength;
        Vitality = vitality;
        Agility = agility;
        Intelligence = intelligence;
        Luck = luck;
        EquipmentCategory = equipmentCategory;
    }

    public int Armor { get; private set; }
    public int MagicArmor { get; private set; }
    public EquipmentCategory EquipmentCategory { get; private set; }
    public int Strength { get; private set; }
    public int Vitality { get; private set; }
    public int Agility { get; private set; }
    public int Intelligence { get; private set; }
    public int Luck { get; private set; }

    public override string ToString()
    {
        return $"[{Rarity.ToString()} {EquipmentCategory.ToString()}] {Name} ({Worth}$)";
    }
}