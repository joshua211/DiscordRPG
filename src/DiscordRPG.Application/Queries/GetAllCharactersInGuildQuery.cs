using DiscordRPG.Common;

namespace DiscordRPG.Application.Queries;

public class GetAllCharactersInGuildQuery : Query<IEnumerable<Character>>
{
    public GetAllCharactersInGuildQuery(Identity guildId)
    {
        GuildId = guildId;
    }

    public Identity GuildId { get; private set; }
}