namespace DiscordRPG.Core.ValueObjects;

public class ActivityData
{
    public uint PlayerLevel { get; set; }
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
}