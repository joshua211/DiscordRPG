using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.DomainServices.Generators;

public interface IEncounterGenerator
{
    Encounter CreateDungeonEncounter(Dungeon dungeon);
}