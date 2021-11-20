namespace DiscordRPG.Core.ValueObjects;

public class Class : IAttributeModifier
{
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

    public float StrengthModifier { get; init; } = 0;
    public float VitalityModifier { get; init; } = 0;
    public float AgilityModifier { get; init; } = 0;
    public float IntelligenceModifier { get; init; } = 0;
    public float LuckModifier { get; init; } = 0;
}