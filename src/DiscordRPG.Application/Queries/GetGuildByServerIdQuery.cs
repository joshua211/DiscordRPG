using DiscordRPG.Common;

namespace DiscordRPG.Application.Queries;

public class GetGuildByServerIdQuery : Query<Guild>
{
    public GetGuildByServerIdQuery(DiscordId guildId)
    {
        GuildId = guildId;
    }

    public DiscordId GuildId { get; private set; }
}