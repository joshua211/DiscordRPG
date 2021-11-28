namespace DiscordRPG.Core.ValueObjects;

public class Race : IAttributeModifier
{
    public Race(string raceName)
    {
        RaceName = raceName;
    }

    public string RaceName { get; private set; }
    public string Description { get; set; }
    public float StrengthModifier { get; init; } = 1;
    public float VitalityModifier { get; init; } = 1;
    public float AgilityModifier { get; init; } = 1;
    public float IntelligenceModifier { get; init; } = 1;
    public float LuckModifier { get; init; } = 1;
}