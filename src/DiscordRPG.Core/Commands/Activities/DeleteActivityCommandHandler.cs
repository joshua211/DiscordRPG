using DiscordRPG.Core.Entities;
using DiscordRPG.Core.Events;
using MediatR;

namespace DiscordRPG.Core.Commands.Activities;

public class DeleteActivityCommandHandler : CommandHandler<DeleteActivityCommand>
{
    private readonly IRepository<Activity> repository;

    public DeleteActivityCommandHandler(IMediator mediator, IRepository<Activity> repository) : base(mediator)
    {
        this.repository = repository;
    }

    public override async Task<ExecutionResult> Handle(DeleteActivityCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            await repository.DeleteAsync(request.Id, cancellationToken);

            await PublishAsync(new ActivityDeleted(request.Id), cancellationToken);

            return ExecutionResult.Success();
        }
        catch (Exception e)
        {
            return ExecutionResult.Failure(e.Message);
        }
    }
}