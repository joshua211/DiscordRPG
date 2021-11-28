using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;
using Serilog;

namespace DiscordRPG.Core.Commands.Dungeons;

public class CalculateAdventureResultCommandHandler : CommandHandler<CalculateAdventureResultCommand>
{
    private readonly IRepository<Character> characterRepository;

    public CalculateAdventureResultCommandHandler(IMediator mediator, ILogger logger,
        IRepository<Character> characterRepository) : base(mediator, logger)
    {
        this.characterRepository = characterRepository;
    }

    public override async Task<ExecutionResult> Handle(CalculateAdventureResultCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.Here().Debug("Handling {Name}", request.GetType().Name);
            //TODO calculate result somewhere
            var result = new DungeonResult(new List<Wound>()
            {
                new Wound("Cut to the dick", 1)
            }, new List<Item>()
            {
                new Item("Lucky Gold Coin", "A seemingly ordinary gold coin, with an inscription you can barely read",
                    Rarity.Legendary, 1)
            });

            var character = request.Character;
            character.Wounds.AddRange(result.Wounds);
            character.Inventory.AddRange(result.Items);
            await characterRepository.UpdateAsync(character, cancellationToken);

            await PublishAsync(new AdventureResultCalculated(result, character, request.Dungeon), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Debug(e, "Handling failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}