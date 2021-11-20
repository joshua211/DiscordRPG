namespace DiscordRPG.Core.ValueObjects;

public interface IAttributeModifier
{
    float StrengthModifier { get; }
    float VitalityModifier { get; }
    float AgilityModifier { get; }
    float IntelligenceModifier { get; }
    float LuckModifier { get; }
}