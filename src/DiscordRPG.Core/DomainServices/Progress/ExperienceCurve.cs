namespace DiscordRPG.Core.DomainServices.Progress;

public class ExperienceCurve : IExperienceCurve
{
    public static readonly int BaseExp = 100;
    public static readonly float BaseDifficulty = 1.25f;

    public ulong GetRequiredExperienceForLevel(uint level) =>
        (ulong) Math.Floor(BaseExp * Math.Pow(level, BaseDifficulty));
}