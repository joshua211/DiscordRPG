using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;

namespace DiscordRPG.Domain.DomainServices.Generators;

public interface IExperienceGenerator
{
    ulong GenerateExperienceFromEncounter(Encounter encounter);
}