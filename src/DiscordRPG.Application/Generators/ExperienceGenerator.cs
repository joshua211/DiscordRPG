using DiscordRPG.Core.DomainServices;
using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.Core.DomainServices.Progress;

namespace DiscordRPG.Application.Generators;

public class ExperienceGenerator : GeneratorBase, IExperienceGenerator
{
    private readonly IExperienceCurve experienceCurve;

    public ExperienceGenerator(IExperienceCurve experienceCurve)
    {
        this.experienceCurve = experienceCurve;
    }

    public ulong GenerateExperienceFromEncounter(Encounter encounter)
    {
        var expForLevel = experienceCurve.GetRequiredExperienceForLevel(encounter.Level);
        return (expForLevel / (uint) random.Next(2, 10));
    }
}