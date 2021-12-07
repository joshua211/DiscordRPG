using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.DomainServices.Progress;

public interface IProgressService
{
    ApplyWoundsResult ApplyWounds(ref Character character, List<Wound> wounds);
    ApplyExperienceResult ApplyExperience(ref Character character, ulong experience);
    ApplyItemsResult ApplyItems(ref Character character, List<Item> items);
}