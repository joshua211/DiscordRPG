using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Character.ValueObjects;

namespace DiscordRPG.Domain.DomainServices.Generators;

public interface IWoundGenerator
{
    IEnumerable<Wound> GenerateWounds(Character character, Encounter encounter);
}