using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.DomainServices;
using DiscordRPG.Core.DomainServices.Progress;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;
using Serilog;
using Weighted_Randomizer;

namespace DiscordRPG.Core.Commands.Dungeons;

public class CalculateAdventureResultCommandHandler : CommandHandler<CalculateAdventureResultCommand>
{
    private readonly IAdventureResultService adventureService;
    private readonly IRepository<Character> characterRepository;
    private readonly IProgressService progressService;

    public CalculateAdventureResultCommandHandler(IMediator mediator, ILogger logger,
        IRepository<Character> characterRepository, IAdventureResultService adventureService,
        IProgressService progressService) : base(mediator, logger)
    {
        this.characterRepository = characterRepository;
        this.adventureService = adventureService;
        this.progressService = progressService;
    }

    public override async Task<ExecutionResult> Handle(CalculateAdventureResultCommand request,
        CancellationToken cancellationToken)
    {
        logger.Here().Debug("Handling {Name}", request.GetType().Name);
        try
        {
            var hasFailed = false;
            var result =
                adventureService.CalculateAdventureResult(request.Character, request.Dungeon, request.Duration);

            var charBefore = request.Character.DeepCopy<Character>();
            var character = request.Character;
            var woundsBefore = request.Character.Wounds.DeepCopy<List<Wound>>();

            var woundResult = progressService.ApplyWounds(ref character, result.Wounds);
            ApplyExperienceResult expResult;
            ApplyItemsResult itemResult;

            if (woundResult.HasDied)
            {
                expResult = new ApplyExperienceResult(0, 0, 0);
                itemResult = new ApplyItemsResult(new List<Item>());
                var selector = new DynamicWeightedRandomizer<bool>
                {
                    {true, (int) request.Dungeon.DungeonLevel},
                    {false, request.Character.Luck}
                };
                var hasActuallyDied = selector.NextWithReplacement();
                if (!hasActuallyDied)
                {
                    hasFailed = true;
                    while (character.CurrentHealth <= 0)
                        character.Wounds.Remove(character.Wounds.Last());
                    await characterRepository.UpdateAsync(character, cancellationToken);
                }
                else
                {
                    logger.Here().Debug("Character {ID} has died from {Wound}", character.ID, woundResult.FinalWound);
                    await characterRepository.DeleteAsync(character.ID, cancellationToken);
                    await PublishAsync(new CharacterDied(charBefore, woundResult.FinalWound!), cancellationToken);
                }
            }
            else
            {
                expResult = progressService.ApplyExperience(ref character, result.Experience);
                itemResult = progressService.ApplyItems(ref character, result.Items);
                await characterRepository.UpdateAsync(character, cancellationToken);
            }

            await PublishAsync(
                new AdventureResultCalculated(character, hasFailed, request.Dungeon, expResult, itemResult, woundResult,
                    result.Encounters, woundsBefore),
                cancellationToken);

            if (expResult.TotalLevelsGained > 0)
            {
                await PublishAsync(
                    new LevelGained(charBefore.Level.CurrentLevel, character.Level.CurrentLevel, character.ID),
                    cancellationToken);
            }

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Debug(e, "Handling failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}