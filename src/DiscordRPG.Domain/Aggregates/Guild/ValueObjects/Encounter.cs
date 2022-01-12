using DiscordRPG.Domain.Entities.Character.ValueObjects;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Aggregates.Guild.ValueObjects;

public class Encounter : ValueObject
{
    public Encounter(Damage damage, int health, int armor, int magicArmor, uint level, int agility, int strength,
        int vitality, int intelligence, int luck)
    {
        Damage = damage;
        Health = health;
        Armor = armor;
        MagicArmor = magicArmor;
        Level = level;
        Agility = agility;
        Strength = strength;
        Vitality = vitality;
        Intelligence = intelligence;
        Luck = luck;
    }

    public Damage Damage { get; private set; }
    public int Health { get; set; }
    public int Armor { get; private set; }
    public int MagicArmor { get; private set; }
    public uint Level { get; private set; }
    public int Agility { get; private set; }
    public int Strength { get; }
    public int Vitality { get; private set; }
    public int Intelligence { get; private set; }
    public int Luck { get; private set; }
}