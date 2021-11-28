using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;
using Serilog;

namespace DiscordRPG.Core.Commands.Dungeons;

public class DeleteDungeonCommandHandler : CommandHandler<DeleteDungeonCommand>
{
    private readonly IRepository<Dungeon> repository;

    public DeleteDungeonCommandHandler(IMediator mediator, ILogger logger, IRepository<Dungeon> repository) : base(
        mediator, logger)
    {
        this.repository = repository;
    }

    public override async Task<ExecutionResult> Handle(DeleteDungeonCommand request,
        CancellationToken cancellationToken)
    {
        logger.Here().Debug("Handling {Name}", request.GetType().Name);
        try
        {
            await repository.DeleteAsync(request.Id, cancellationToken);

            await PublishAsync(new DungeonDeleted(request.Id), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Debug(e, "Handling failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}