using DiscordRPG.Common;

namespace DiscordRPG.Application.Queries;

public class GetShopByGuildIdQuery : Query<Shop>
{
    public GetShopByGuildIdQuery(Identity guildId)
    {
        GuildId = guildId;
    }

    public Identity GuildId { get; private set; }
}