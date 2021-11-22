using DiscordRPG.Core.Repositories;
using MediatR;

namespace DiscordRPG.Core.Commands;

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
        await activityRepository.SaveActivityAsync(request.Activity, cancellationToken);

        return ExecutionResult.Success();
    }
}