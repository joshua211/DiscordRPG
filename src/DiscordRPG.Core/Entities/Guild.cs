namespace DiscordRPG.Core.Entities;

public class Guild : Entity
{
    public Guild(DiscordId serverId, string guildName, DiscordId guildHallId, DiscordId dungeonHallId,
        DiscordId innChannel)
    {
        ServerId = serverId;
        GuildName = guildName;
        GuildHallId = guildHallId;
        DungeonHallId = dungeonHallId;
        InnChannel = innChannel;
    }

    public DiscordId ServerId { get; private set; }
    public string GuildName { get; private set; }
    public DiscordId GuildHallId { get; private set; }
    public DiscordId DungeonHallId { get; private set; }
    public DiscordId InnChannel { get; set; }
}