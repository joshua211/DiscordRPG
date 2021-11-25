using DiscordRPG.Core.Events;
using DiscordRPG.Core.Repositories;
using MediatR;

namespace DiscordRPG.Core.Commands.Activities;

public class CreateActivityCommandHandler : CommandHandler<CreateActivityCommand>
{
    private readonly IActivityRepository activityRepository;

    public CreateActivityCommandHandler(IMediator mediator, IActivityRepository activityRepository) : base(mediator)
    {
        this.activityRepository = activityRepository;
    }

    public override async Task<ExecutionResult> Handle(CreateActivityCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await activityRepository.SaveActivityAsync(request.Activity, cancellationToken);

            await PublishAsync(new ActivityCreated(request.Activity), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            return ExecutionResult.Failure(e.Message);
        }
    }
}