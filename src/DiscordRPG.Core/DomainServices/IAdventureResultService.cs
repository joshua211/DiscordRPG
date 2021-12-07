using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.DomainServices;

public interface IAdventureResultService
{
    AdventureResult CalculateAdventureResult(Character character, Dungeon dungeon, ActivityDuration duration);
}