using EventFlow.Exceptions;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Entities.Character.ValueObjects;

public class CharacterRace : ValueObject
{
    public CharacterRace(string name, string description, float strengthModifier, float vitalityModifier,
        float agilityModifier, float intelligenceModifier, float luckModifier)
    {
        if (string.IsNullOrEmpty(name))
            DomainError.With(nameof(name));
        if (string.IsNullOrEmpty(description))
            DomainError.With(nameof(description));

        Name = name;
        Description = description;
        StrengthModifier = strengthModifier;
        VitalityModifier = vitalityModifier;
        AgilityModifier = agilityModifier;
        IntelligenceModifier = intelligenceModifier;
        LuckModifier = luckModifier;
    }

    public string Name { get; private set; }
    public string Description { get; set; }
    public float StrengthModifier { get; init; } = 1;
    public float VitalityModifier { get; init; } = 1;
    public float AgilityModifier { get; init; } = 1;
    public float IntelligenceModifier { get; init; } = 1;
    public float LuckModifier { get; init; } = 1;
}