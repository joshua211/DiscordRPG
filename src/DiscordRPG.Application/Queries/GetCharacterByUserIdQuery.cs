using DiscordRPG.Common;

namespace DiscordRPG.Application.Queries;

public class GetCharacterByUserIdQuery : Query<Character>
{
    public GetCharacterByUserIdQuery(ulong userId, ulong guildId)
    {
        UserId = userId;
        GuildId = guildId;
    }

    public ulong UserId { get; init; }
    public ulong GuildId { get; init; }
}