using DiscordRPG.Core.Entities;

namespace DiscordRPG.Core.Events;

public class GuildCreated : DomainEvent
{
    public GuildCreated(Guild guild)
    {
        Guild = guild;
    }

    public Guild Guild { get; private set; }
}