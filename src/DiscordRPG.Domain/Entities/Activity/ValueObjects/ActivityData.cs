using DiscordRPG.Domain.Aggregates.Guild;
using DiscordRPG.Domain.Aggregates.Guild.ValueObjects;
using DiscordRPG.Domain.Entities.Character;
using DiscordRPG.Domain.Entities.Dungeon;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Entities.Activity.ValueObjects;

public class ActivityData : ValueObject
{
    public ActivityData(uint playerLevel, GuildId guildId, ChannelId channelId,
        DungeonId dungeonId, CharacterId userId)
    {
        PlayerLevel = playerLevel;
        GuildId = guildId;
        ChannelId = channelId;
        DungeonId = dungeonId;
        UserId = userId;
    }

    public uint PlayerLevel { get; set; }
    public GuildId GuildId { get; set; }
    public ChannelId ChannelId { get; set; }
    public DungeonId DungeonId { get; set; }
    public CharacterId UserId { get; set; }
}