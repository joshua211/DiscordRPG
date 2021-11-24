namespace DiscordRPG.Core.ValueObjects;

public class Dungeon
{
    public Dungeon(ulong dungeonChannelId, uint dungeonLevel, Rarity rarity, string name)
    {
        DungeonChannelId = dungeonChannelId;
        DungeonLevel = dungeonLevel;
        Rarity = rarity;
        Name = name;
    }

    public ulong DungeonChannelId { get; private set; }
    public uint DungeonLevel { get; private set; }
    public Rarity Rarity { get; private set; }
    public string Name { get; private set; }
}