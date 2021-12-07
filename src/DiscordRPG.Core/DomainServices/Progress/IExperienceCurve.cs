namespace DiscordRPG.Core.DomainServices.Progress;

public interface IExperienceCurve
{
    ulong GetRequiredExperienceForLevel(uint level);
}