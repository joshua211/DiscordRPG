using DiscordRPG.Domain.Entities.Character.Enums;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Entities.Character.ValueObjects;

public class Damage : ValueObject
{
    public Damage(DamageType damageType, int value)
    {
        DamageType = damageType;
        Value = value;
    }

    public DamageType DamageType { get; private set; }
    public int Value { get; private set; }
}