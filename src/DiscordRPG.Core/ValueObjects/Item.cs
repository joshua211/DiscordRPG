using MongoDB.Bson.Serialization.Attributes;

namespace DiscordRPG.Core.ValueObjects;

public class Item
{
    [BsonConstructor]
    public Item(string name, string description, Rarity rarity, int worth, uint level)
    {
        Name = name;
        Description = description;
        Rarity = rarity;
        Worth = worth;
        Level = level;
    }

    public string Name { get; init; }
    public string Description { get; init; }
    public Rarity Rarity { get; init; }
    public int Worth { get; private set; }
    public uint Level { get; private set; }

    public override string ToString()
    {
        return $"[{Rarity.ToString()} {GetType().Name}] {Name} (Lvl: {Level}, $:{Worth})";
    }

    public string GetItemCode() => $"{(int) Rarity}-{Name}{Level}";
}