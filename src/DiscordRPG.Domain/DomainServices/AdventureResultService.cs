using DiscordRPG.Common;
using DiscordRPG.Domain.Aggregates.Activity.Enums;
using DiscordRPG.Domain.Aggregates.Character.ValueObjects;
using DiscordRPG.Domain.Aggregates.Dungeon;
using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using DiscordRPG.Domain.DomainServices.Generators;
using Weighted_Randomizer;

namespace DiscordRPG.Domain.DomainServices;

public class AdventureResultService : IAdventureResultService
{
    private readonly IEncounterGenerator encounterGenerator;
    private readonly IExperienceGenerator experienceGenerator;
    private readonly IItemGenerator itemGenerator;
    private readonly IWoundGenerator woundGenerator;

    public AdventureResultService(IEncounterGenerator encounterGenerator, IExperienceGenerator experienceGenerator,
        IItemGenerator itemGenerator, IWoundGenerator woundGenerator)
    {
        this.encounterGenerator = encounterGenerator;
        this.experienceGenerator = experienceGenerator;
        this.itemGenerator = itemGenerator;
        this.woundGenerator = woundGenerator;
    }

    public void Calculate(GuildAggregate aggregate, CharacterId characterId, DungeonId dungeonId,
        ActivityDuration duration, TransactionContext context)
    {
        var character = aggregate.Characters.FirstOrDefault(c => c.Id.Value == characterId.Value);
        if (character is null)
            throw new ArgumentException($"No Character with Id {characterId} found");

        var dungeon = aggregate.Dungeons.FirstOrDefault(d => d.Id.Value == dungeonId.Value);
        if (dungeon is null)
            throw new ArgumentException($"No Dungeon with Id {dungeonId} found");

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
            items.AddRange(itemGenerator.GenerateItems(character, dungeon));
            exp += experienceGenerator.GenerateExperienceFromEncounter(encounter);
        }

        var result = new AdventureResult(wounds, items, encounters, exp);
        ;
        aggregate.PublishAdventureResult(result, characterId, dungeonId, context);
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