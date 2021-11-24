namespace DiscordRPG.Core.Entities;

public class Guild : Entity
{
    public Guild(ulong serverId, string guildName, ulong guildHallId, ulong dungeonHallId, List<ulong> characters,
        List<Dungeon> dungeons)
    {
        ServerId = serverId;
        GuildName = guildName;
        GuildHallId = guildHallId;
        DungeonHallId = dungeonHallId;
        Characters = characters;
        Dungeons = dungeons;
    }

    public ulong ServerId { get; private set; }
    public string GuildName { get; private set; }
    public ulong GuildHallId { get; private set; }
    public ulong DungeonHallId { get; private set; }
    public List<ulong> Characters { get; private set; }
    public List<Dungeon> Dungeons { get; private set; }
}