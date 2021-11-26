using DiscordRPG.Common.Extensions;
using MediatR;
using Serilog;

namespace DiscordRPG.Common;

public abstract class CommandHandler<T> : IRequestHandler<T, ExecutionResult> where T : IRequest<ExecutionResult>
{
    protected readonly ILogger logger;
    private readonly IMediator mediator;

    protected CommandHandler(IMediator mediator, ILogger logger)
    {
        this.mediator = mediator;
        this.logger = logger.WithContext(GetType());
    }

    public abstract Task<ExecutionResult> Handle(T request, CancellationToken cancellationToken);

    public async Task PublishAsync(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.Here().Debug("Publishing event {Name}", domainEvent.GetType().Name);
        await mediator.Publish(domainEvent, cancellationToken);
    }
}