namespace DiscordRPG.Core.DomainServices;

public class Encounter
{
    public Encounter(Damage damage, int health, int armor, int magicArmor, uint level)
    {
        Damage = damage;
        Health = health;
        Armor = armor;
        MagicArmor = magicArmor;
        Level = level;
    }

    public Damage Damage { get; private set; }
    public int Health { get; set; }
    public int Armor { get; private set; }
    public int MagicArmor { get; private set; }
    public uint Level { get; private set; }
}