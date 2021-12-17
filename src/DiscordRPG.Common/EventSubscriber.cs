using MediatR;

namespace DiscordRPG.Common;

public abstract class EventSubscriber<T> : IDomainEventSubscriber<T> where T : DomainEvent, INotification
{
    public abstract Task Handle(T domainEvent, CancellationToken cancellationToken);
}

public interface IDomainEventSubscriber<in T> : INotificationHandler<T> where T : DomainEvent, INotification
{
}