using DiscordRPG.Common;

namespace DiscordRPG.Application.Queries;

public class GetCharacterByUserIdQuery : Query<Character>
{
    public GetCharacterByUserIdQuery(DiscordId userId, Identity guildId)
    {
        UserId = userId;
        GuildId = guildId;
    }

    public DiscordId UserId { get; init; }
    public string GuildId { get; init; }
}