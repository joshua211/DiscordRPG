using DiscordRPG.Core.Repositories;
using MediatR;

namespace DiscordRPG.Core.Commands.Activities;

public class DeleteActivityCommandHandler : CommandHandler<DeleteActivityCommand>
{
    private readonly IActivityRepository repository;

    public DeleteActivityCommandHandler(IMediator mediator, IActivityRepository repository) : base(mediator)
    {
        this.repository = repository;
    }

    public override async Task<ExecutionResult> Handle(DeleteActivityCommand request,
        CancellationToken cancellationToken)
    {
        await repository.DeleteActivityAsync(request.Id, cancellationToken);

        return ExecutionResult.Success();
    }
}