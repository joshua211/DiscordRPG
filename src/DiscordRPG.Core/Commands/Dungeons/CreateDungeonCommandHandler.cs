using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;
using Serilog;

namespace DiscordRPG.Core.Commands.Dungeons;

public class CreateDungeonCommandHandler : CommandHandler<CreateDungeonCommand>
{
    private readonly IRepository<Dungeon> dungeonRepository;

    public CreateDungeonCommandHandler(IMediator mediator, IRepository<Dungeon> dungeonRepository, ILogger logger) :
        base(mediator, logger)
    {
        this.dungeonRepository = dungeonRepository;
    }

    public override async Task<ExecutionResult> Handle(CreateDungeonCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.Here().Debug("Handling {Name}", request.GetType().Name);

            await dungeonRepository.SaveAsync(request.Dungeon, cancellationToken);

            await PublishAsync(new DungeonCreated(request.Dungeon, request.ActivityDuration), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Debug(e, "Handling failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}