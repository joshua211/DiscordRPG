using DiscordRPG.Common;
using DiscordRPG.Core.Events;

namespace DiscordRPG.Application.Subscribers;

public class GuildDeletedSubscriber : EventSubscriber<GuildDeleted>
{
    public override Task Handle(GuildDeleted domainEvent, CancellationToken cancellationToken)
    {
        //TODO delete characters of that guild
        throw new NotImplementedException();
    }
}