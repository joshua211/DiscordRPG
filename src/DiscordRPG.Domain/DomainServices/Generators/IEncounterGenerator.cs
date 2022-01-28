using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using DiscordRPG.Domain.Entities.Dungeon;

namespace DiscordRPG.Domain.DomainServices.Generators;

public interface IEncounterGenerator
{
    Encounter CreateDungeonEncounter(Dungeon dungeon);
}