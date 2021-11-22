using DiscordRPG.Common;

namespace DiscordRPG.Application.Queries;

public class GetGuildQuery : Query<Guild>
{
    public GetGuildQuery(ulong guildId)
    {
        GuildId = guildId;
    }

    public ulong GuildId { get; private set; }
}