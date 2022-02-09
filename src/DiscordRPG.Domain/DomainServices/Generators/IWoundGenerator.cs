using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;

namespace DiscordRPG.Domain.DomainServices.Generators;

public interface IWoundGenerator
{
    IEnumerable<Wound> GenerateWounds(Character character, Encounter encounter);
}