namespace DiscordRPG.Domain.Services;

public class ExperienceCurve : IExperienceCurve
{
    public const int BaseExp = 100;
    public const float BaseDifficulty = 1.25f;

    public ulong GetRequiredExperienceForLevel(uint level) =>
        (ulong) Math.Floor(BaseExp * Math.Pow(level, BaseDifficulty));
}

public interface IExperienceCurve
{
    ulong GetRequiredExperienceForLevel(uint level);
}