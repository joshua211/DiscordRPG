namespace DiscordRPG.Core.Entities;

public class Guild : Entity
{
    public Guild(DiscordId serverId, string guildName, DiscordId guildHallId, DiscordId dungeonHallId)
    {
        ServerId = serverId;
        GuildName = guildName;
        GuildHallId = guildHallId;
        DungeonHallId = dungeonHallId;
    }

    public DiscordId ServerId { get; private set; }
    public string GuildName { get; private set; }
    public DiscordId GuildHallId { get; private set; }
    public DiscordId DungeonHallId { get; private set; }
}