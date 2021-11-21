using MediatR;

namespace DiscordRPG.Common;

public abstract class EventSubscriber<T> : INotificationHandler<T> where T : DomainEvent, INotification
{
    public abstract Task Handle(T domainEvent, CancellationToken cancellationToken);
}