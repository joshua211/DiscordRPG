using EventFlow.Exceptions;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Entities.Character.ValueObjects;

public class CharacterClass : ValueObject
{
    public CharacterClass(string name, string description, byte baseStrength, byte baseVitality, byte baseAgility,
        byte baseIntelligence, byte baseLuck, float strengthModifier, float vitalityModifier, float agilityModifier,
        float intelligenceModifier, float luckModifier)
    {
        if (string.IsNullOrEmpty(name))
            DomainError.With(nameof(name));
        if (string.IsNullOrEmpty(description))
            DomainError.With(nameof(description));
        Name = name;
        Description = description;
        BaseStrength = baseStrength;
        BaseVitality = baseVitality;
        BaseAgility = baseAgility;
        BaseIntelligence = baseIntelligence;
        BaseLuck = baseLuck;
        StrengthModifier = strengthModifier;
        VitalityModifier = vitalityModifier;
        AgilityModifier = agilityModifier;
        IntelligenceModifier = intelligenceModifier;
        LuckModifier = luckModifier;
    }

    public string Name { get; private set; }
    public string Description { get; set; }
    public byte BaseStrength { get; init; } = 10;
    public byte BaseVitality { get; init; } = 10;
    public byte BaseAgility { get; init; } = 10;
    public byte BaseIntelligence { get; init; } = 10;
    public byte BaseLuck { get; init; } = 10;

    public float StrengthModifier { get; init; } = 1;
    public float VitalityModifier { get; init; } = 1;
    public float AgilityModifier { get; init; } = 1;
    public float IntelligenceModifier { get; init; } = 1;
    public float LuckModifier { get; init; } = 1;
}