using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using DiscordRPG.Domain.DomainServices.Generators;
using DiscordRPG.Domain.Services;

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
        return (expForLevel / (uint) random.Next(4, 7));
    }
}