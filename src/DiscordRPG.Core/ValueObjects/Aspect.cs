namespace DiscordRPG.Core.ValueObjects;

public class Aspect
{
    public Aspect(string dungeonPrefix, IEnumerable<string> itemPrefixes)
    {
        ItemPrefixes = itemPrefixes;
        DungeonPrefix = dungeonPrefix;
    }

    public IEnumerable<string> ItemPrefixes { get; private set; }
    public string DungeonPrefix { get; private set; }
}