namespace DiscordRPG.Core.ValueObjects;

public class Race : IAttributeModifier
{
    public Race(string raceName)
    {
        RaceName = raceName;
    }

    public string RaceName { get; private set; }

    public float StrengthModifier { get; init; } = 0;
    public float VitalityModifier { get; init; } = 0;
    public float AgilityModifier { get; init; } = 0;
    public float IntelligenceModifier { get; init; } = 0;
    public float LuckModifier { get; init; } = 0;
}