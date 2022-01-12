using EventFlow.Exceptions;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Entities.Character.ValueObjects;

public class Wound : ValueObject
{
    public Wound(string description, int damageValue)
    {
        if (string.IsNullOrEmpty(description))
            DomainError.With(nameof(description));
        if (damageValue <= 0)
            DomainError.With(nameof(damageValue));
        Description = description;
        DamageValue = damageValue;
    }

    public string Description { get; private set; }
    public int DamageValue { get; set; }

    public override string ToString()
    {
        return $"{Description} (-{DamageValue})";
    }
}