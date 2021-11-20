namespace DiscordRPG.Core.ValueObjects;

public class Wound
{
    public Wound(string description, int damageValue)
    {
        Description = description;
        DamageValue = damageValue;
    }

    public string Description { get; private set; }
    public int DamageValue { get; private set; }
}