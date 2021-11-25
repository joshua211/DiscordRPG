using MediatR;

namespace DiscordRPG.Common;

public abstract class CommandHandler<T> : IRequestHandler<T, ExecutionResult> where T : IRequest<ExecutionResult>
{
    private readonly IMediator mediator;

    protected CommandHandler(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public abstract Task<ExecutionResult> Handle(T request, CancellationToken cancellationToken);

    public async Task PublishAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await mediator.Publish(domainEvent, cancellationToken);
    }
}