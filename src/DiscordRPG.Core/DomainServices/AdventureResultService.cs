using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.Core.Entities;
using Weighted_Randomizer;

namespace DiscordRPG.Core.DomainServices;

public class AdventureResultService : IAdventureResultService
{
    private readonly IEncounterGenerator encounterGenerator;
    private readonly IExperienceGenerator experienceGenerator;
    private readonly IItemGenerator itemGenerator;
    private readonly IWoundGenerator woundGenerator;

    public AdventureResultService(IWoundGenerator woundGenerator, IItemGenerator itemGenerator,
        IExperienceGenerator experienceGenerator, IEncounterGenerator encounterGenerator)
    {
        this.woundGenerator = woundGenerator;
        this.itemGenerator = itemGenerator;
        this.experienceGenerator = experienceGenerator;
        this.encounterGenerator = encounterGenerator;
    }

    public AdventureResult CalculateAdventureResult(Character character, Dungeon dungeon, ActivityDuration duration)
    {
        var wounds = new List<Wound>();
        var items = new List<Item>();
        var encounters = new List<Encounter>();
        ulong exp = 0;
        var numEncounters = GetRandomNumberOfEncounters(duration);

        for (int i = 0; i < numEncounters; i++)
        {
            var encounter = encounterGenerator.CreateDungeonEncounter(dungeon);
            encounters.Add(encounter);
            wounds.AddRange(woundGenerator.GenerateWounds(character, encounter));
            items.AddRange(itemGenerator.GenerateItems(dungeon));
            exp += experienceGenerator.GenerateExperienceFromEncounter(encounter);
        }

        var itemsToRemove = new List<Item>();
        var existing = new List<Item>();
        foreach (var item in items)
        {
            if (existing.All(i => i.GetItemCode() != item.GetItemCode()))
                existing.Add(item);
            else
            {
                var existingItem = items.First(i => i.GetItemCode() == item.GetItemCode());
                existingItem.Amount += item.Amount;
                itemsToRemove.Add(item);
            }
        }

        itemsToRemove.ForEach(i => items.Remove(i));

        return new AdventureResult(wounds, items, exp, encounters);
    }

    private int GetRandomNumberOfEncounters(ActivityDuration duration)
    {
        var selector = new DynamicWeightedRandomizer<int>();
        switch (duration)
        {
            case ActivityDuration.Quick:
                selector.Add(0, 3);
                selector.Add(1, 10);
                break;
            case ActivityDuration.Short:
                selector.Add(1, 10);
                selector.Add(2, 7);
                selector.Add(3, 1);
                break;
            case ActivityDuration.Medium:
                selector.Add(1, 4);
                selector.Add(2, 10);
                selector.Add(3, 5);
                selector.Add(4, 1);
                break;
            case ActivityDuration.Long:
                selector.Add(2, 8);
                selector.Add(3, 10);
                selector.Add(4, 6);
                break;
            case ActivityDuration.ExtraLong:
                selector.Add(2, 3);
                selector.Add(3, 8);
                selector.Add(4, 10);
                selector.Add(5, 3);
                break;
        }

        return selector.NextWithReplacement();
    }
}