using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.DomainServices.Progress;

public class ProgressService : IProgressService
{
    private readonly IExperienceCurve experienceCurve;

    public ProgressService(IExperienceCurve experienceCurve)
    {
        this.experienceCurve = experienceCurve;
    }

    public ApplyWoundsResult ApplyWounds(ref Character character, List<Wound> wounds)
    {
        foreach (var wound in wounds)
        {
            character.Wounds.Add(wound);
            if (character.CurrentHealth <= 0)
            {
                return new ApplyWoundsResult(true, wound, wounds, character.CurrentHealth);
            }
        }

        return new ApplyWoundsResult(false, null, wounds, character.CurrentHealth);
    }

    public ApplyExperienceResult ApplyExperience(ref Character character, ulong experience)
    {
        uint totalLevels = 0;

        character.Level.CurrentExp += experience;
        while (character.Level.CurrentExp >= character.Level.RequiredExp)
        {
            character.Level.CurrentLevel++;
            character.Level.CurrentExp -= character.Level.RequiredExp;
            character.Level.RequiredExp =
                experienceCurve.GetRequiredExperienceForLevel(character.Level.CurrentLevel);
            totalLevels++;
        }

        return new ApplyExperienceResult(totalLevels, experience, character.Level.CurrentLevel);
    }

    public ApplyItemsResult ApplyItems(ref Character character, List<Item> items)
    {
        character.Inventory.AddRange(items);

        return new ApplyItemsResult(items);
    }
}