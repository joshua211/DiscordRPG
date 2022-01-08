using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace DiscordRPG.Core.ValueObjects;

public class Aspect
{
    public Aspect(string dungeonPrefix, Dictionary<Rarity, IEnumerable<string>> itemPrefixes)
    {
        ItemPrefixes = itemPrefixes;
        DungeonPrefix = dungeonPrefix;
    }

    [BsonConstructor]
    public Aspect()
    {
    }

    [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfArrays)]
    public Dictionary<Rarity, IEnumerable<string>> ItemPrefixes { get; set; }

    public string DungeonPrefix { get; set; }
}