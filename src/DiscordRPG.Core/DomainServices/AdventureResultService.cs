using DiscordRPG.Core.DomainServices.Generators;
using DiscordRPG.Core.Entities;
using DiscordRPG.WeightedRandom;

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
        ulong exp = 0;
        var numEncounters = GetRandomNumberOfEncounters(duration);

        for (int i = 0; i < numEncounters; i++)
        {
            var encounter = encounterGenerator.CreateDungeonEncounter(dungeon);
            wounds.AddRange(woundGenerator.GenerateWounds(character, encounter));
            items.AddRange(itemGenerator.GenerateItems(dungeon));
            exp += experienceGenerator.GenerateExperienceFromEncounter(encounter);
        }

        return new AdventureResult(wounds, items, exp);
    }

    private int GetRandomNumberOfEncounters(ActivityDuration duration)
    {
        var selector = new DynamicRandomSelector<int>();
        switch (duration)
        {
            case ActivityDuration.Quick:
                selector.Add(0, 0.3f);
                selector.Add(1, 1);
                break;
            case ActivityDuration.Short:
                selector.Add(1, 1);
                selector.Add(2, 0.7f);
                selector.Add(3, 0.1f);
                break;
            case ActivityDuration.Medium:
                selector.Add(1, 0.4f);
                selector.Add(2, 1f);
                selector.Add(3, 0.5f);
                selector.Add(4, 0.1f);
                break;
            case ActivityDuration.Long:
                selector.Add(2, 0.8f);
                selector.Add(3, 1f);
                selector.Add(4, 0.6f);
                break;
            case ActivityDuration.ExtraLong:
                selector.Add(2, 0.3f);
                selector.Add(3, 0.8f);
                selector.Add(4, 1f);
                selector.Add(5, 0.3f);
                break;
        }

        return selector.Build().SelectRandomItem();
    }
}