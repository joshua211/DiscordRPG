using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;
using Serilog;

namespace DiscordRPG.Core.Commands.Dungeons;

public class DecreaseExplorationCommandHandler : CommandHandler<DecreaseExplorationCommand>
{
    private readonly IRepository<Dungeon> dungeonRepository;

    public DecreaseExplorationCommandHandler(IMediator mediator, ILogger logger, IRepository<Dungeon> dungeonRepository)
        : base(mediator, logger)
    {
        this.dungeonRepository = dungeonRepository;
    }

    public override async Task<ExecutionResult> Handle(DecreaseExplorationCommand request,
        CancellationToken cancellationToken)
    {
        logger.Here().Debug("Handling {Name}", request.GetType().Name);
        try
        {
            var dungeon = request.Dungeon;
            dungeon.ExplorationsLeft--;
            await dungeonRepository.UpdateAsync(dungeon, cancellationToken);

            await PublishAsync(new DungeonExplorationsDecreased(dungeon), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Debug(e, "Handling failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}