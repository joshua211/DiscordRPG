using MediatR;

namespace DiscordRPG.Common;

public abstract class CommandHandler<T> : INotificationHandler<T> where T : INotification
{
    protected readonly IMediator mediator;

    protected CommandHandler(IMediator mediator)
    {
        this.mediator = mediator;
    }

    public abstract Task Handle(T command, CancellationToken cancellationToken);
}