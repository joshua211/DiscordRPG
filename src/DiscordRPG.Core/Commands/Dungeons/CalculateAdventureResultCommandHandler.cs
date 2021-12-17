using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.DomainServices;
using DiscordRPG.Core.DomainServices.Progress;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;
using Serilog;

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
            var result =
                adventureService.CalculateAdventureResult(request.Character, request.Dungeon, request.Duration);

            var character = request.Character;
            var woundResult = progressService.ApplyWounds(ref character, result.Wounds);
            var expResult = progressService.ApplyExperience(ref character, result.Experience);
            var itemResult = progressService.ApplyItems(ref character, result.Items);

            if (woundResult.HasDied)
            {
                logger.Here().Debug("Character {ID} has died from {Wound}", character.ID, woundResult.FinalWound);
                await characterRepository.DeleteAsync(character.ID, cancellationToken);
                await PublishAsync(new CharacterDied(character, woundResult.FinalWound!), cancellationToken);
            }
            else
            {
                await characterRepository.UpdateAsync(character, cancellationToken);
            }

            await PublishAsync(
                new AdventureResultCalculated(character, request.Dungeon, expResult, itemResult, woundResult,
                    result.Encounters),
                cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Debug(e, "Handling failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}