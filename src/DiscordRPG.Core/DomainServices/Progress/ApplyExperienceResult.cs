namespace DiscordRPG.Core.DomainServices.Progress;

public class ApplyExperienceResult
{
    public ApplyExperienceResult(uint totalLevelsGained, ulong totalExperienceGained, uint newLevel)
    {
        TotalLevelsGained = totalLevelsGained;
        TotalExperienceGained = totalExperienceGained;
        NewLevel = newLevel;
    }

    public uint TotalLevelsGained { get; private set; }
    public ulong TotalExperienceGained { get; private set; }
    public uint NewLevel { get; private set; }
}