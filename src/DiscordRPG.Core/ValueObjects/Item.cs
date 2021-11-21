using MongoDB.Bson.Serialization.Attributes;

namespace DiscordRPG.Core.ValueObjects;

public class Item
{
    [BsonConstructor]
    public Item(string name, string description, Rarity rarity, int worth)
    {
        Name = name;
        Description = description;
        Rarity = rarity;
        Worth = worth;
    }

    public string Name { get; init; }
    public string Description { get; init; }
    public Rarity Rarity { get; init; }
    public int Worth { get; private set; }
}