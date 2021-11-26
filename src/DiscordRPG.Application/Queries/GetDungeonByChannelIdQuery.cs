using DiscordRPG.Common;

namespace DiscordRPG.Application.Queries;

public class GetDungeonByChannelIdQuery : Query<Dungeon>
{
    public GetDungeonByChannelIdQuery(DiscordId channelId)
    {
        ChannelId = channelId;
    }

    public DiscordId ChannelId { get; private set; }
}