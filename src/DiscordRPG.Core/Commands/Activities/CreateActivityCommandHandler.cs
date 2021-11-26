using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;

namespace DiscordRPG.Core.Commands.Activities;

public class CreateActivityCommandHandler : CommandHandler<CreateActivityCommand>
{
    private readonly IRepository<Activity> activityRepository;

    public CreateActivityCommandHandler(IMediator mediator, IRepository<Activity> activityRepository) : base(mediator)
    {
        this.activityRepository = activityRepository;
    }

    public override async Task<ExecutionResult> Handle(CreateActivityCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await activityRepository.SaveAsync(request.Activity, cancellationToken);

            await PublishAsync(new ActivityCreated(request.Activity), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            return ExecutionResult.Failure(e.Message);
        }
    }
}