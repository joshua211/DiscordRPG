using DiscordRPG.Domain.Aggregates.Character.Enums;
using EventFlow.Exceptions;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Aggregates.Character.ValueObjects;

public class StatusEffect : ValueObject
{
    public StatusEffect(StatusEffectType statusEffectType, float modifier, string name)
    {
        if (string.IsNullOrEmpty(name))
            throw DomainError.With(nameof(name));
        if (modifier == 0)
            throw DomainError.With(nameof(modifier));

        StatusEffectType = statusEffectType;
        Modifier = modifier;
        Name = name;
    }

    public StatusEffectType StatusEffectType { get; private set; }
    public float Modifier { get; private set; }
    public string Name { get; private set; }
}