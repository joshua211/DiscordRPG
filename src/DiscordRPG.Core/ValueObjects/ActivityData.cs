namespace DiscordRPG.Core.ValueObjects;

public class ActivityData
{
    public uint PlayerLevel { get; set; }
    public ulong ServerId { get; set; }
    public ulong ChannelId { get; set; }
    public ulong ThreadId { get; set; }
    public string DungeonId { get; set; }
    public ulong UserId { get; set; }
}