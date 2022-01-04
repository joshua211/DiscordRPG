namespace DiscordRPG.Core.ValueObjects;

public class Aspect
{
    public Aspect(string dungeonPrefix, Dictionary<Rarity, IEnumerable<string>> itemPrefixes)
    {
        ItemPrefixes = itemPrefixes;
        DungeonPrefix = dungeonPrefix;
    }

    public Dictionary<Rarity, IEnumerable<string>> ItemPrefixes { get; private set; }
    public string DungeonPrefix { get; private set; }
}