using DiscordRPG.Domain.Aggregates.Character;
using DiscordRPG.Domain.Aggregates.Dungeon;
using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Aggregates.Activity.ValueObjects;

public class ActivityData : ValueObject
{
    public ActivityData(uint playerLevel, ActivityId ActivityId, ChannelId channelId,
        DungeonId dungeonId, CharacterId userId)
    {
        PlayerLevel = playerLevel;
        ActivityId = ActivityId;
        ChannelId = channelId;
        DungeonId = dungeonId;
        UserId = userId;
    }

    public uint PlayerLevel { get; set; }
    public ActivityId ActivityId { get; set; }
    public ChannelId ChannelId { get; set; }
    public DungeonId DungeonId { get; set; }
    public CharacterId UserId { get; set; }
}