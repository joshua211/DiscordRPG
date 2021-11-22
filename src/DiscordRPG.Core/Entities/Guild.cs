namespace DiscordRPG.Core.Entities;

public class Guild : Entity
{
    public Guild(ulong serverId, string guildName, ulong guildHallId, ulong dungeonHallId, List<ulong> characters)
    {
        ServerId = serverId;
        GuildName = guildName;
        GuildHallId = guildHallId;
        DungeonHallId = dungeonHallId;
        Characters = characters;
    }

    public ulong ServerId { get; private set; }
    public string GuildName { get; private set; }
    public ulong GuildHallId { get; private set; }
    public ulong DungeonHallId { get; private set; }
    public List<ulong> Characters { get; private set; }
}