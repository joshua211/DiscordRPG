namespace DiscordRPG.Core.Entities;

public class Guild : Entity
{
    public Guild(ulong serverId, string guildName, ulong guildHallId, ulong dungeonHallId)
    {
        ServerId = serverId;
        GuildName = guildName;
        GuildHallId = guildHallId;
        DungeonHallId = dungeonHallId;
    }

    public ulong ServerId { get; private set; }
    public string GuildName { get; private set; }
    public ulong GuildHallId { get; private set; }
    public ulong DungeonHallId { get; private set; }
}