using DiscordRPG.Domain.Enums;

namespace DiscordRPG.Domain.Entities.Dungeon.ValueObjects;

public class Aspect
{
    public Aspect(string dungeonPrefix, Dictionary<Rarity, IEnumerable<string>> itemPrefixes)
    {
        ItemPrefixes = itemPrefixes;
        DungeonPrefix = dungeonPrefix;
    }

    public Dictionary<Rarity, IEnumerable<string>> ItemPrefixes { get; set; }

    public string DungeonPrefix { get; set; }
}