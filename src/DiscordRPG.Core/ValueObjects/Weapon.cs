using MongoDB.Bson.Serialization.Attributes;

namespace DiscordRPG.Core.ValueObjects;

public class Weapon : Equipment
{
    [BsonConstructor]
    public Weapon(string name, string description, Rarity rarity, int armor, int magicArmor, int strength, int vitality,
        int agility, int intelligence, int luck, int worth, CharacterAttribute damageAttribute, DamageType damageType,
        int damageValue, EquipmentCategory equipmentCategory, uint level) : base(name, description, rarity, armor,
        magicArmor,
        strength, vitality, agility, intelligence,
        luck, worth, equipmentCategory, level)
    {
        DamageAttribute = damageAttribute;
        DamageType = damageType;
        DamageValue = damageValue;
    }

    public CharacterAttribute DamageAttribute { get; private set; }
    public DamageType DamageType { get; private set; }
    public int DamageValue { get; private set; }

    public override string ToString()
    {
        return $"[{Rarity.ToString()} Weapon] {Name} (Lvl: {Level}, $:{Worth})";
    }
}