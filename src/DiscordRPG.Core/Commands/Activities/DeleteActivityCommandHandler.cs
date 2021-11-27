using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;
using Serilog;

namespace DiscordRPG.Core.Commands.Activities;

public class DeleteActivityCommandHandler : CommandHandler<DeleteActivityCommand>
{
    private readonly IRepository<Activity> repository;

    public DeleteActivityCommandHandler(IMediator mediator, IRepository<Activity> repository, ILogger logger) : base(
        mediator, logger)
    {
        this.repository = repository;
    }

    public override async Task<ExecutionResult> Handle(DeleteActivityCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger.Here().Debug("Handling {Name}", request.GetType().Name);
            await repository.DeleteAsync(request.Id, cancellationToken);

            await PublishAsync(new ActivityDeleted(request.Id), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Here().Debug(e, "Handle failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}