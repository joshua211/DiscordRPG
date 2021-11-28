using MongoDB.Bson.Serialization.Attributes;

namespace DiscordRPG.Core.ValueObjects;

public class Class : IAttributeModifier
{
    [BsonConstructor]
    public Class(string className)
    {
        ClassName = className;
    }

    public string ClassName { get; private set; }

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