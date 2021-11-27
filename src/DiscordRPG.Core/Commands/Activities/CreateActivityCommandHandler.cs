using DiscordRPG.Common.Extensions;
using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;
using Serilog;

namespace DiscordRPG.Core.Commands.Activities;

public class CreateActivityCommandHandler : CommandHandler<CreateActivityCommand>
{
    private readonly IRepository<Activity> activityRepository;

    public CreateActivityCommandHandler(IMediator mediator, IRepository<Activity> activityRepository, ILogger logger) :
        base(mediator, logger)
    {
        this.activityRepository = activityRepository;
    }

    public override async Task<ExecutionResult> Handle(CreateActivityCommand request,
        CancellationToken cancellationToken)
    {
        logger.Here().Debug("Handling {Name}", request.GetType().Name);
        try
        {
            await activityRepository.SaveAsync(request.Activity, cancellationToken);

            await PublishAsync(new ActivityCreated(request.Activity), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            logger.Here().Debug(e, "Handling failed");
            return ExecutionResult.Failure(e.Message);
        }
    }
}