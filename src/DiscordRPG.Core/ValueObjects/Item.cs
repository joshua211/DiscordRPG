using System.Text.RegularExpressions;
using MongoDB.Bson.Serialization.Attributes;

namespace DiscordRPG.Core.ValueObjects;

public class Item
{
    protected static readonly Regex WhitespaceRegex = new Regex(@"\s+");

    public Item(string name, string description, Rarity rarity, int worth, uint level, int amount, bool isUsable)
    {
        Name = name;
        Description = description;
        Rarity = rarity;
        Worth = worth;
        Level = level;
        Amount = amount;
        IsUsable = isUsable;
    }

    [BsonConstructor]
    public Item()
    {
    }

    public string Name { get; set; }
    public int Amount { get; set; }
    public string Description { get; set; }
    public Rarity Rarity { get; set; }
    public int Worth { get; set; }
    public uint Level { get; set; }
    public bool IsUsable { get; set; }

    public override string ToString()
    {
        return $"[{Rarity.ToString()} {GetType().Name}] {Name} (Lvl: {Level} | {Worth}$)";
    }

    public virtual string GetItemCode() => $"{Rarity}-{WhitespaceRegex.Replace(Name, "")}{Level}|{Worth}";
}