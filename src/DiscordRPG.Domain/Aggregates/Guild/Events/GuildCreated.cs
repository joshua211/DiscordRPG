using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using EventFlow.Aggregates;

namespace DiscordRPG.Domain.Aggregates.Guild.Events;

public class GuildCreated : AggregateEvent<GuildAggregate, GuildId>
{
    public GuildCreated(GuildId id, GuildName name, ChannelId guildHallId, ChannelId dungeonHallId, ChannelId innId)
    {
        Id = id;
        Name = name;
        GuildHallId = guildHallId;
        DungeonHallId = dungeonHallId;
        InnId = innId;
    }

    public GuildId Id { get; private set; }
    public GuildName Name { get; private set; }
    public ChannelId GuildHallId { get; private set; }
    public ChannelId DungeonHallId { get; private set; }
    public ChannelId InnId { get; private set; }
}