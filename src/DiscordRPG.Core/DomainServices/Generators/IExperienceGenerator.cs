namespace DiscordRPG.Core.DomainServices.Generators;

public interface IExperienceGenerator
{
    ulong GenerateExperienceFromEncounter(Encounter encounter);
}