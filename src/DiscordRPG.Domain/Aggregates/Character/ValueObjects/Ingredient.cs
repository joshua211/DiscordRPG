using DiscordRPG.Domain.Enums;
using EventFlow.ValueObjects;

namespace DiscordRPG.Domain.Aggregates.Character.ValueObjects;

public class Ingredient : ValueObject
{
    public Ingredient(Rarity rarity, string name, uint level, byte amount)
    {
        Rarity = rarity;
        Name = name;
        Level = level;
        Amount = amount;
    }

    public Rarity Rarity { get; private set; }
    public string Name { get; private set; }
    public uint Level { get; private set; }
    public byte Amount { get; private set; }
}