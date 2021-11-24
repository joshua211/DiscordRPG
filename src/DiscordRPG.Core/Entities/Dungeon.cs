namespace DiscordRPG.Core.Entities;

public class Dungeon : Entity
{
    public Dungeon(ulong guildId, ulong dungeonChannelId, uint dungeonLevel, Rarity rarity, string name)
    {
        GuildId = guildId;
        DungeonChannelId = dungeonChannelId;
        DungeonLevel = dungeonLevel;
        Rarity = rarity;
        Name = name;
    }

    public ulong GuildId { get; private set; }
    public ulong DungeonChannelId { get; private set; }
    public uint DungeonLevel { get; private set; }
    public Rarity Rarity { get; private set; }
    public string Name { get; private set; }
}