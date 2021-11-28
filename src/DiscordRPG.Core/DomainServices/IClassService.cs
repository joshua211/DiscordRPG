namespace DiscordRPG.Core.DomainServices;

public interface IClassService
{
    Class GetClass(int id);

    IEnumerable<(int id, Class @class)> GetAllClasses();
    IEnumerable<string> GetStrengths(Class charClass);
    IEnumerable<string> GetWeaknesses(Class charClass);
}