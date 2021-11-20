using MediatR;

namespace DiscordRPG.Common;

public abstract class EventHandler<T> : INotificationHandler<T> where T : INotification
{
    public abstract Task Handle(T notification, CancellationToken cancellationToken);
}