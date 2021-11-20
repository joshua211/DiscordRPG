namespace DiscordRPG.Core.ValueObjects;

public class Damage
{
    public Damage(DamageType damageType, int value)
    {
        DamageType = damageType;
        Value = value;
    }

    public DamageType DamageType { get; private set; }
    public int Value { get; private set; }
}