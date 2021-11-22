using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Events;

public class GuildDeleted : DomainEvent
{
    public GuildDeleted(Guild guild)
    {
        Guild = guild;
    }

    public Guild Guild { get; private set; }
}