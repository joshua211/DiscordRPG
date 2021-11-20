namespace DiscordRPG.Core.ValueObjects;

public class Weapon : Equipment
{
    public Weapon(string name, string description, Rarity rarity, int armor, int magicArmor, int strength, int vitality,
        int agility, int intelligence, int luck, int worth, CharacterAttribute damageAttribute, DamageType damageType,
        int damageValue) : base(name, description, rarity, armor, magicArmor, strength, vitality, agility, intelligence,
        luck, worth)
    {
        DamageAttribute = damageAttribute;
        DamageType = damageType;
        DamageValue = damageValue;
    }

    public CharacterAttribute DamageAttribute { get; private set; }
    public DamageType DamageType { get; private set; }
    public int DamageValue { get; private set; }
}