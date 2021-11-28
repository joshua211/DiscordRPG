namespace DiscordRPG.Core.DomainServices;

public interface IRaceService
{
    Race GetRace(int id);

    IEnumerable<(int id, Race race)> GetAllRaces();

    IEnumerable<string> GetStrengths(Race race);

    IEnumerable<string> GetWeaknesses(Race race);
}