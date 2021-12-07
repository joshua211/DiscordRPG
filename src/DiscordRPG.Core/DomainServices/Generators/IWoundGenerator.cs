using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.DomainServices.Generators;

public interface IWoundGenerator
{
    IEnumerable<Wound> GenerateWounds(Character character, Encounter encounter);
}