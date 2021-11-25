using DiscordRPG.Common;

namespace DiscordRPG.Application.Queries;

public class GetDungeonByChannelIdQuery : Query<Dungeon>
{
    public GetDungeonByChannelIdQuery(ulong channelId)
    {
        ChannelId = channelId;
    }

    public ulong ChannelId { get; private set; }
}