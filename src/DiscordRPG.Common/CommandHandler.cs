using MediatR;

namespace DiscordRPG.Common;

public abstract class CommandHandler<T> : INotificationHandler<T> where T : INotification
{
    public abstract Task Handle(T command, CancellationToken cancellationToken);
}